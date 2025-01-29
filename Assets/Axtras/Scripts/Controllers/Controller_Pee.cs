using UnityEngine;
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
    [SerializeField] private float dehydrationThreshold = 20f; // Pee meter value where dehydration starts
    [SerializeField] private float dehydrationAmount = 0.5f;    // Amount of dehydration per second after pee meter is empty
    [SerializeField] private float maxDehydration = 30f;       // Maximum dehydration before increasing kidney stone chance
    [SerializeField] private float kidneyStoneChance = 0.1f;    // Base chance of getting a kidney stone
    [SerializeField] private float maxKidneyStoneChance = 0.5f; // Maximum chance of getting a kidney stone

    [Header("Hydration Settings")]
    [SerializeField] private bool isHydrating = false;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem peePS;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] peeClips;

    [Header("UI Settings")]
    [SerializeField] private Image peeImage;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        currPeeAmount = maxPeeAmount;
    }
    
    private void Update() {
        UpdatePeeAmount();
        UpdateUI();
        CheckDehydration();
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
                Debug.Log("Player's bladder is full!");
            }
        }

        currPeeAmount = Mathf.Clamp(currPeeAmount, 0f, maxPeeAmount);
    }
    public void AddPeeAmount(float amount) {
        currPeeAmount += amount;

        // Reset dehydration when pee amount is added
        if (currPeeAmount > 0f) {
            dehydrationAmount = 0f;
        }
    }
    public bool GetIsPeeFull() {
        return currPeeAmount == maxPeeAmount;
    }

    public void SetIsPeeing(bool active) {
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
        // If dehydration reaches max, increase kidney stone chance
        if (dehydrationAmount >= maxDehydration) {
            kidneyStoneChance += Time.deltaTime * kidneyStoneChance;

            // Clamp kidney stone chance to max value
            kidneyStoneChance = Mathf.Clamp(kidneyStoneChance, 0f, maxKidneyStoneChance);

            // Check if player gets a kidney stone
            if (Random.value <= kidneyStoneChance) {
                Debug.Log("Player got a kidney stone!");
                // Implement logic for dealing with kidney stone (e.g., pain, effects on gameplay)
            }
        }
    }
    private void IncreaseDehydration(float amount) {
        // Increase dehydration up to maxDehydration
        dehydrationAmount += amount;
        dehydrationAmount = Mathf.Clamp(dehydrationAmount, 0f, maxDehydration);
    }

    private void UpdateUI() {
        peeImage.fillAmount = currPeeAmount / maxPeeAmount;
    }
}