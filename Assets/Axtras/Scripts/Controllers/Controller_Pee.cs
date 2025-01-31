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

    [Header("Dehydration Settings")]
    [SerializeField] private bool isDehydrated = false;
    [SerializeField] private float gameOverInTime = 0f;
    [SerializeField] private float maxGameOverInTime = 10f;

    [Header("Stones Settings")]
    [SerializeField] private int stonesCurrentCount = 0;
    [SerializeField] private int stonesAcquiredCount = 0;
    [SerializeField] private int stonesPassedCount = 0;
    [SerializeField] private float timeToKidneyStone = 0f;
    [SerializeField] private float maxTimeToKidneyStone = 10f;

    [Header("Hydration Settings")]
    [SerializeField] private bool isHydrating = false;

    [Header("QTE Settings")]
    [SerializeField] private int qtePressCount = 0;
    [SerializeField] private int qteRequiredPresses = 30;
    [SerializeField] private bool allowQTE = false;
    [SerializeField] private KeyCode qteKey = KeyCode.E;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem peePS;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] peeClips;
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
        HandleControls();
        PeeLoop();
        DehydrationLoop();
    }
    
    private void HandleControls() {
        if (Input.GetMouseButtonDown(0)) {
            isPeeing = true;
            peePS.Play();
        }
        else if (Input.GetMouseButtonUp(0)) {
            isPeeing = false;
            peePS.Stop();
        }

        if (allowQTE && Input.GetKeyDown(qteKey)) {
            qtePressCount++;
            if (qtePressCount >= qteRequiredPresses) {
                PassKidneyStone();
            }
        }
    }

    private void PeeLoop() {
        if (isPeeing) {
            if (currPeeAmount > 0f) {
                currPeeAmount -= Time.deltaTime * peeEmptyRate;

                Helper.Instance.PlayRandAudio(audioSource, peeClips);
            }
            else if (currPeeAmount <= 0f) {
                currPeeAmount = 0f;
                isDehydrated = true;
                Manager_UI.Instance.SetDehydratedUI(true);
                // Add dehydration effects - speed reduction, pee slower, etc.
            }
        }

        Manager_UI.Instance.SetPeeAmountUI(currPeeAmount/maxPeeAmount);
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
    
    private void DehydrationLoop() {
        if (isDehydrated) {
            // Start kidney stone timer if player is dehydrated and still peeing
            if (isPeeing) {
                timeToKidneyStone += timeToKidneyStone * Time.deltaTime;
                timeToKidneyStone = Mathf.Clamp(timeToKidneyStone, 0.5f, maxTimeToKidneyStone);
            }
            
            if (timeToKidneyStone >= maxTimeToKidneyStone) {
                Debug.Log("Player got a kidney stone!");

                peePS.Stop();
                allowQTE = true;
                qtePressCount = 0;
                timeToKidneyStone = 0f;
                stonesCurrentCount++;
                stonesAcquiredCount++;

                // Kidney stone effects - speed reduction, pee slower, etc.
                maxPeeAmount -= 15f;
                Controller_Player.Instance.ControlSpeedMoveAndLook(0.8f);
                var emission = peePS.emission;
                emission.rateOverTime = peePS.emission.rateOverTime.constant * 0.8f;
            }

            // Start game over timer if player is dehydrated
            gameOverInTime += Time.deltaTime;
            if (gameOverInTime >= maxGameOverInTime) {
                Manager_Effects.Instance.ResetDehydratedEffects();
                Manager_UI.Instance.SetDehydratedUI(false);
                Manager_UI.Instance.GameOver();
            }
        }
    }
    private void ResetDehydration() {
        Debug.Log("Player is no longer dehydrated.");

        isDehydrated = false;

        Manager_UI.Instance.SetDehydratedUI(false);
        Manager_Effects.Instance.ResetDehydratedEffects();
        
        timeToKidneyStone = 0f;
        gameOverInTime = 0f;
    }

    private void PassKidneyStone() {
        Debug.Log("PassKidneyStone");
                         
        qtePressCount = 0;
        stonesCurrentCount--;
        stonesPassedCount++;

        // Allow QTE to pass kidney stone again
        if (stonesCurrentCount <= 0) {
            allowQTE = false;
        }

        // Kidney stone passing temp boosts - speed, pee, etc.
        maxPeeAmount += 5f;
        Controller_Player.Instance.ControlSpeedMoveAndLook(1.4f);
        var emission = peePS.emission;
        emission.rateOverTime = peePS.emission.rateOverTime.constant * 1.4f;
    }
    
    public int GetStonesPassedCount() {
        return stonesPassedCount;
    }
    public int GetStonesAcquiredCount() {
        return stonesAcquiredCount;
    }
}