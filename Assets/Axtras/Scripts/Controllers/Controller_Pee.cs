using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private Coroutine kidneyStoneEffectCoroutine;
    private Coroutine kidneyStonePassBoostCoroutine;

    [Header("Hydration Settings")]
    [SerializeField] private bool isHydrating = false;

    [Header("QTE Settings")]
    [SerializeField] private int qtePressCount = 0;
    [SerializeField] private int qteRequiredPresses = 30;
    [SerializeField] private bool allowQTE = false;
    private KeyCode qteKey;

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
        var peePerc = currPeeAmount / maxPeeAmount;

        if (isPeeing) {
            if (currPeeAmount > 0f) {
                currPeeAmount -= Time.deltaTime * peeEmptyRate;

                Helper.Instance.PlayRandAudio(audioSource, peeClips);
            }
            else if (currPeeAmount <= 0f) {
                currPeeAmount = 0f;
                isDehydrated = true;
            }
        }

        Manager_UI.Instance.SetPeeAmountUI(peePerc);
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
                Manager_Effects.Instance.DehydrationEffects(timeToKidneyStone/maxTimeToKidneyStone);
            }
            
            if (timeToKidneyStone >= maxTimeToKidneyStone) {
                Debug.Log("Player got a kidney stone!");

                allowQTE = true;
                GetRandKey();

                qtePressCount = 0;
                timeToKidneyStone = 0f;
                stonesCurrentCount++;
                stonesAcquiredCount++;
                Manager_UI.Instance.SetKidneyStoneUI(stonesCurrentCount);

                // Kidney stone getting perm effects
                ApplyKidneyStoneEffects();
            }

            // Start game over timer if player is dehydrated
            gameOverInTime += Time.deltaTime;
            Manager_Effects.Instance.UpdateGameOverEffects(gameOverInTime/maxGameOverInTime);
            if (gameOverInTime >= maxGameOverInTime) {
                gameOverInTime = maxGameOverInTime;
                timeToKidneyStone = 0f;

                Manager_Effects.Instance.DehydrationEffects(0f);
                Manager_Effects.Instance.ResetGameOverEffects();

                Manager_UI.Instance.GameOver();
            }
        }
    }
    private void ResetDehydration() {
        Debug.Log("Player is no longer dehydrated.");

        isDehydrated = false;
        
        gameOverInTime = 0f;
        timeToKidneyStone = 0f;

        Manager_Effects.Instance.ResetGameOverEffects();
        Manager_Effects.Instance.DehydrationEffects(0f);
    }

    private void PassKidneyStone() {
        Debug.Log("PassKidneyStone");
                         
        qtePressCount = 0;
        stonesCurrentCount--;
        stonesPassedCount++;
        
        Manager_UI.Instance.SetKidneyStoneUI(stonesCurrentCount);

        // Allow QTE to pass kidney stone again
        if (stonesCurrentCount <= 0) {
            allowQTE = false;
            Manager_UI.Instance.SetQTEKeyUI("");
        }

        // Kidney stone passing temp boost
        ApplyKidneyStonePassingBoost();
    }

    private KeyCode GetRandKey() {
        KeyCode[] keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        List<KeyCode> validKeyCodes = new ();
        foreach (KeyCode keyCode in keyCodes) {
            if (
                (keyCode >= KeyCode.A && keyCode <= KeyCode.Z && keyCode != KeyCode.E && keyCode != KeyCode.F && keyCode != KeyCode.G) || 
                (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9) || 
                (keyCode >= KeyCode.F1 && keyCode <= KeyCode.F12)
            ) {
            validKeyCodes.Add(keyCode);
            }
        }

        qteKey = validKeyCodes[Random.Range(0, validKeyCodes.Count)];
        Manager_UI.Instance.SetQTEKeyUI(qteKey.ToString());

        return qteKey;
    }

    private void ApplyKidneyStoneEffects() {
        if (kidneyStoneEffectCoroutine != null) 
            StopCoroutine(kidneyStoneEffectCoroutine);
        
        kidneyStoneEffectCoroutine = StartCoroutine(TemporaryEffect(-15f, 0.8f, 0.8f, -1f));
    }
    private void ApplyKidneyStonePassingBoost() {
        if (kidneyStonePassBoostCoroutine != null) 
            StopCoroutine(kidneyStonePassBoostCoroutine);
        
        kidneyStonePassBoostCoroutine = StartCoroutine(TemporaryEffect(5f, 1.4f, 1.4f, 3f));
    }
    private IEnumerator TemporaryEffect(float peeAmountChange, float speedMultiplier, float peeRateMultiplier, float duration) {
        // Apply changes and clamp values
        maxPeeAmount = Mathf.Clamp(maxPeeAmount + peeAmountChange, 0f, 150f);
        Controller_Player.Instance.ControlSpeedMoveAndLook(Mathf.Clamp(speedMultiplier, 0.1f, 1.3f));

        var emission = peePS.emission;
        emission.rateOverTime = Mathf.Clamp(emission.rateOverTime.constant * peeRateMultiplier, 10, 150);

        if (duration == -1f) yield break;

        yield return new WaitForSeconds(duration);

        // Revert changes after duration
        maxPeeAmount = Mathf.Clamp(maxPeeAmount - peeAmountChange, 0f, 150f);
        Controller_Player.Instance.ControlSpeedMoveAndLook(Mathf.Clamp(1 / speedMultiplier, 0.1f, 1.3f));

        emission = peePS.emission;
        emission.rateOverTime = Mathf.Clamp(emission.rateOverTime.constant / peeRateMultiplier, 10, 150);
    }
    
    public int GetStonesPassedCount() {
        return stonesPassedCount;
    }
    public int GetStonesAcquiredCount() {
        return stonesAcquiredCount;
    }
}