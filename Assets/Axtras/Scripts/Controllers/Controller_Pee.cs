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
    [SerializeField] private float peeDurationForMaxEffect = 3f;
    [SerializeField] private float minFadeMul = 0.1f;
    [SerializeField] private float maxFadeMul = 1.5f;
    float peeStartTime;
    float peeDuration; 
    float peePerc; 
    float fadePerc; 
    float fadeMul; 

    [Header("Dehydration Settings")]
    [SerializeField] private bool isDehydrated = false;

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
    private KeyCode qteKey;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem peePS;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource peeAudioSource;
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

        Manager_UI.Instance.SetPeeAmountUI(currPeeAmount / maxPeeAmount);
        Manager_UI.Instance.SetKidneyStoneUI(stonesCurrentCount);
        Manager_UI.Instance.SetQTEKeyUI("");
    }
    
    private void Update() {
        var isShowingTutorials = Manager_Tutorials.Instance.GetIfShowingTutorials();
        if (isShowingTutorials) 
            return;
            
        HandleControls();
        PeeLoop();
        DehydrationLoop();
    }
    
    private void HandleControls() {
        if (Input.GetMouseButtonDown(0)) {
            isPeeing = true;
            peeStartTime = Time.time;
            peePS.Play();
            peeAudioSource.Play();
        }
        else if (Input.GetMouseButtonUp(0)) {
            isPeeing = false;
            peePS.Stop();
            peeAudioSource.Stop();
            Manager_Effects.Instance.ResetDehydrationEffect();
            CalcPeeVol();
        }

        if (allowQTE && Input.GetKeyDown(qteKey)) {
            qtePressCount++;
            if (qtePressCount >= qteRequiredPresses) {
                PassKidneyStone();
            }
        }
    }

    private void PeeLoop() {
        peePerc = currPeeAmount / maxPeeAmount;
        fadePerc = peeDuration / peeDurationForMaxEffect;
        fadeMul = Mathf.Lerp(minFadeMul, maxFadeMul, fadePerc);

        if (isPeeing) {
            if (currPeeAmount > 0f) {
                currPeeAmount -= Time.deltaTime * peeEmptyRate;

                peeDuration += Time.deltaTime;
            }
            else if (currPeeAmount <= 0f) {
                currPeeAmount = 0f;
                isDehydrated = true;
            }
        }
        else {
            peeDuration = 0f;
        }

        Manager_UI.Instance.SetPeeAmountUI(peePerc);
    }
    public void AddPeeAmount(float amount) {
        currPeeAmount += amount;

        // Clamp pee amount
        if (currPeeAmount > maxPeeAmount) {
            currPeeAmount = maxPeeAmount;
        }

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
                timeToKidneyStone += Time.deltaTime;
                timeToKidneyStone = Mathf.Clamp(timeToKidneyStone, 0.5f, maxTimeToKidneyStone);
                var shakeIntensity = Mathf.Lerp(0.2f, 1.5f, timeToKidneyStone / maxTimeToKidneyStone);
                
                // Debug.Log($"DehydrationLoop\ntimeToKidneyStone: {timeToKidneyStone}\nshakeIntensity: {shakeIntensity}");
                Manager_Effects.Instance.UpdateDehydrationEffects(timeToKidneyStone/maxTimeToKidneyStone);
                Manager_Effects.Instance.ShakeDehydrationEffect(shakeIntensity);
            }
            
            if (timeToKidneyStone >= maxTimeToKidneyStone) {
                Debug.Log("Player got a kidney stone!");

                allowQTE = true;
                GetRandKey();

                // Stop shaking effect and trigger explosion effect
                Manager_Effects.Instance.ExplodeDehydrationEffect();

                qtePressCount = 0;
                timeToKidneyStone = 0f;
                stonesCurrentCount++;
                stonesAcquiredCount++;

                Manager_UI.Instance.SetKidneyStoneUI(stonesCurrentCount);
                Manager_SaveLoad.Instance.SaveStatData("totalKidneyStonesCreated", "add", 1);

                // Kidney stone getting perm effects
                Manager_Effects.Instance.SetGotStoneEffect();
            }
        }
    }
    private void ResetDehydration() {
        // Debug.Log("Player is no longer dehydrated.");

        isDehydrated = false;
        
        timeToKidneyStone = 0f;

        Manager_Effects.Instance.ResetVisualEffects();
        Manager_Effects.Instance.UpdateDehydrationEffects(0f);
    }

    private void PassKidneyStone() {
        Debug.Log("PassKidneyStone");
                         
        qtePressCount = 0;
        stonesCurrentCount--;
        stonesPassedCount++;
        
        Manager_SaveLoad.Instance.SaveStatData("totalKidneyStonesPassed", "add", 1);
        Manager_UI.Instance.SetKidneyStoneUI(stonesCurrentCount);

        // Allow QTE to pass kidney stone again
        if (stonesCurrentCount <= 0) {
            allowQTE = false;
            Manager_UI.Instance.SetQTEKeyUI("");
        }

        // Kidney stone passing temp boost
        Manager_Effects.Instance.SetPassStoneEffect();
    }

    private void CalcPeeVol() {
        var peedForTime = Time.time - peeStartTime;
        var peeRate = peePS.emission.rateOverTime.constant;
        var volOfPee = peedForTime * peeRate;

        Manager_SaveLoad.Instance.SaveStatData("peedAmount", "set", (int)volOfPee);
    }

    private KeyCode GetRandKey() {
        KeyCode[] keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        List<KeyCode> validKeyCodes = new ();
        foreach (KeyCode keyCode in keyCodes) {
            if (
                keyCode >= KeyCode.A && keyCode <= KeyCode.Z && 
                keyCode != KeyCode.E && 
                keyCode != KeyCode.F && 
                keyCode != KeyCode.W && keyCode != KeyCode.A && keyCode != KeyCode.S && keyCode != KeyCode.D && 
                keyCode != KeyCode.G
            ) {
            validKeyCodes.Add(keyCode);
            }
        }

        qteKey = validKeyCodes[Random.Range(0, validKeyCodes.Count)];
        Manager_UI.Instance.SetQTEKeyUI(qteKey.ToString());

        return qteKey;
    }
    public float GetFadeMul() {
        return fadeMul;
    }
    public float GetMaxPeeAmount() {
        return maxPeeAmount;
    }
    public int GetStonesPassedCount() {
        return stonesPassedCount;
    }
    public int GetStonesAcquiredCount() {
        return stonesAcquiredCount;
    }
    
    public void SetMaxPeeAmount(float amount) {
        maxPeeAmount = amount;
    }
}