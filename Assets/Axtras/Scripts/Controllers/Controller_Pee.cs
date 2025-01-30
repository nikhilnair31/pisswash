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
    [SerializeField] private bool isDehydrated = false;
    [SerializeField] private float dehydrationTimer = 0f;
    [SerializeField] private float maxDehydrationTime = 10f;
    [SerializeField] private float dehydrationThreshold = 20f; // Pee meter value where dehydration starts
    [SerializeField] private float dehydrationAmount = 0.5f;    // Amount of dehydration per second after pee meter is empty
    [SerializeField] private float maxDehydration = 30f;       // Maximum dehydration before increasing kidney stone chance
    [SerializeField] private float kidneyStoneChance = 0.1f;    // Base chance of getting a kidney stone
    [SerializeField] private float maxKidneyStoneChance = 0.5f; // Maximum chance of getting a kidney stone

    [Header("Hydration Settings")]
    [SerializeField] private bool isHydrating = false;

    [Header("QTE Settings")]
    [SerializeField] private int qtePressCount = 0;
    [SerializeField] private int qteRequiredPresses = 30;
    [SerializeField] private bool isQTEActive = false;
    [SerializeField] private KeyCode qteKey = KeyCode.E;
    [SerializeField] private bool passedKidneyStone = false;

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
        CheckDehydration();
        UpdateQTE();
        UpdateUI();

        Manager_Effects.Instance.UpdateDehydrationEffects(dehydrationTimer/maxDehydrationTime);
    }
    
    private void UpdatePeeAmount() {
        if (isPeeing && !isHydrating) {
            currPeeAmount -= Time.deltaTime * peeEmptyRate;
        }
        else if (!isPeeing && !isHydrating) {
            currPeeAmount += Time.deltaTime * peeFillRate;
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
                Manager_UI.Instance.SetDehydratedUI(true);
                SetIsPeeing(false);
                
                if (!passedKidneyStone) {
                    StartQTE();
                }
            }

            dehydrationTimer += Time.deltaTime;
            if (dehydrationTimer >= maxDehydrationTime) {
                Manager_Effects.Instance.ResetDehydratedEffects();
                Manager_UI.Instance.SetDehydratedUI(false);
                Manager_UI.Instance.GameOver();
            }
        }
        else {
            if (currPeeAmount <= 0f ) {
                dehydrationAmount += dehydrationAmount * Time.deltaTime;
                dehydrationAmount = Mathf.Clamp(dehydrationAmount, 0f, maxDehydration);
            }
        }
    }
    private void ResetDehydration() {
        Debug.Log("Player is no longer dehydrated.");

        isDehydrated = false;
        passedKidneyStone = false;
        Manager_UI.Instance.SetDehydratedUI(false);
        Manager_Effects.Instance.ResetDehydratedEffects();
        dehydrationAmount = 0.5f;
        kidneyStoneChance = 0.1f;
        dehydrationTimer = 0f;
    }
    public bool GetIfDehydrated() {
        return isDehydrated;
    }

    private void StartQTE() {
        if (isQTEActive) return;

        isQTEActive = true;
        qtePressCount = 0;
        Controller_Player.Instance.ControlCanMoveAndLook(false);
        Manager_Thoughts.Instance.ShowText(
            "[X] to pass kidney stone", 
            -1f,
            Manager_Thoughts.TextPriority.Player
        );
    }
    private void UpdateQTE() {
        if (isQTEActive && Input.GetKeyDown(qteKey)) {
            qtePressCount++;
            if (qtePressCount >= qteRequiredPresses) {
                CompleteQTE();
            }
        }
    }
    private void CompleteQTE() {
        Debug.Log("Kidney stone passed!");
        
        isQTEActive = false;
        passedKidneyStone = true;
        Controller_Player.Instance.ControlCanMoveAndLook(true);
        Manager_Thoughts.Instance.ClearThoughtText(
            Manager_Thoughts.TextPriority.Player
        );
    }

    private void UpdateUI() {
        peeImage.fillAmount = currPeeAmount / maxPeeAmount;
    }
}