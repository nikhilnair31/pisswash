using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Controller_Pee : MonoBehaviour 
{
    #region Vars
    public static Controller_Pee Instance { get; private set; }

    [Header("Pee Settings")]
    [SerializeField] private bool isPeeing = false;
    [SerializeField] private float maxPeeAmount = 100f;
    [SerializeField] private float currPeeAmount;
    [SerializeField] private float peeEmptyRate = 1f;
    [SerializeField] private float peeFillRate = 2f;

    [Header("Dehydration Settings")]
    [SerializeField] private bool isDehydrated = false;
    [SerializeField] private float dehydrationTimer = 0f;
    [SerializeField] private float maxDehydrationTime = 10f;
    [SerializeField] private float dehydrationThreshold = 20f; // Pee meter value where dehydration starts
    [SerializeField] private float dehydrationAmount = 0.5f;    // Amount of dehydration per second after pee meter is empty
    [SerializeField] private float maxDehydration = 30f;       // Maximum dehydration before increasing kidney stone chance
    [SerializeField] private float kidneyStoneChance = 0.1f;    // Base chance of getting a kidney stone
    [SerializeField] private float maxKidneyStoneChance = 0.5f; // Maximum chance of getting a kidney stone
    [SerializeField] private float speedReductionFactor = 0.5f;

    [Header("Hydration Settings")]
    [SerializeField] private bool isHydrating = false;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem peePS;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] peeClips;

    [Header("UI Settings")]
    [SerializeField] private Image peeImage;

    [Header("Effect Settings")]
    [SerializeField] private CinemachineVolumeSettings postProcessVolume;
    private Vignette vignette;
    private LensDistortion lensDistortion;
    private SplitToning splitToning;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        currPeeAmount = maxPeeAmount;
        GetVolumeEffects();
    }
    private void GetVolumeEffects() {
        if (postProcessVolume.Profile.TryGet(out vignette)) {
            vignette.intensity.value = 0f;
        } else {
            Debug.LogError("Vignette not found.");
        }
        if (postProcessVolume.Profile.TryGet(out lensDistortion)) {
            lensDistortion.intensity.value = 0f;
        } else {
            Debug.LogError("LensDistortion not found.");
        }
        if (postProcessVolume.Profile.TryGet(out splitToning)) {
            splitToning.balance.value = -100f;
        } else {
            Debug.LogError("SplitToning not found.");
        }
    }
    
    private void Update() {
        UpdatePeeAmount();
        UpdateUI();
        CheckDehydration();
        UpdateDehydrationEffects();
    }
    
    private void UpdatePeeAmount() {
        if (isPeeing && !isHydrating) {
            currPeeAmount -= Time.deltaTime * peeEmptyRate;

            if (currPeeAmount <= 0f) {
                currPeeAmount = 0f;
                IncreaseDehydration(dehydrationAmount * Time.deltaTime);
            }
        }
        else if (!isPeeing && !isHydrating) {
            currPeeAmount += Time.deltaTime * peeFillRate;
            
            if (currPeeAmount >= maxPeeAmount) {
                currPeeAmount = maxPeeAmount;
                // Debug.Log("Player's bladder is full!");
            }
        }

        currPeeAmount = Mathf.Clamp(currPeeAmount, 0f, maxPeeAmount);
    }
    public void AddPeeAmount(float amount) {
        currPeeAmount += amount;

        // Reset dehydration when pee amount is added
        if (currPeeAmount > 0f) {
            ResetDehydration();
        }
    }
    public bool GetIsPeeFull() {
        return currPeeAmount == maxPeeAmount;
    }

    public void SetIsPeeing(bool active) {
        Debug.Log($"Peeing: {active}");

        isPeeing = active;
        
        if (isPeeing) {
            StartPee();
        }
        else {
            StopPee();
        }
    }
    private void StartPee() {
        peePS.Play();
        Helper.Instance.PlayRandAudio(audioSource, peeClips);
    }
    private void StopPee() {
        peePS.Stop();
    }
    
    private void CheckDehydration() {
        if (dehydrationAmount >= maxDehydration) {
            kidneyStoneChance += Time.deltaTime * kidneyStoneChance;
            kidneyStoneChance = Mathf.Clamp(kidneyStoneChance, 0.1f, maxKidneyStoneChance);

            if (Random.value <= kidneyStoneChance) {
                Debug.Log("Player got a kidney stone!");

                isDehydrated = true;
                SetIsPeeing(false);
                Manager_UI.Instance.SetDehydrated(true);
                // Implement logic for dealing with kidney stone (e.g., pain, effects on gameplay)
            }

            // If maximum dehydration time is reached, trigger game over
            dehydrationTimer += Time.deltaTime;
            if (dehydrationTimer >= maxDehydrationTime) {
                Manager_UI.Instance.GameOver();
            }
        }
    }
    private void UpdateDehydrationEffects() {
        if (vignette != null) 
            vignette.intensity.value = Mathf.Lerp(0f, 0.5f, dehydrationTimer / maxDehydrationTime);
        
        if (lensDistortion != null) 
            lensDistortion.intensity.value = Mathf.Lerp(0f, 0.5f, dehydrationTimer / maxDehydrationTime);
        
        if (splitToning != null) 
            splitToning.balance.value = Mathf.Lerp(-100f, 100f, dehydrationTimer / maxDehydrationTime);
    }
    private void IncreaseDehydration(float amount) {
        // Increase dehydration up to maxDehydration
        dehydrationAmount += amount;
        dehydrationAmount = Mathf.Clamp(dehydrationAmount, 0f, maxDehydration);
    }
    private void ResetDehydration() {
        Debug.Log("Player is no longer dehydrated.");

        isDehydrated = false;
        Manager_UI.Instance.SetDehydrated(false);
        dehydrationAmount = 0.5f;
        kidneyStoneChance = 0.1f;
        dehydrationTimer = 0f;
    }
    public float GetDehydrationFactor() {
        return isDehydrated ? speedReductionFactor : 1f;
    }

    private void UpdateUI() {
        peeImage.fillAmount = currPeeAmount / maxPeeAmount;
    }
}