using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections;

public class Manager_Effects : MonoBehaviour 
{
    #region Vars
    public static Manager_Effects Instance { get; private set; }
    
    private GameObject playerGO;
    private CinemachineCamera cam;
    private ParticleSystem peePS;
    private Sequence stunSequence;
    private Sequence slipSequence;
    private Sequence damageSequence;
    private Coroutine movementMultiplierCoroutine;
    private Coroutine kidneyStoneEffectCoroutine;
    private Coroutine kidneyStonePassBoostCoroutine;
    private Coroutine visionDistortionCoroutine;
    private Coroutine saturationVignetteCoroutine;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] slapClips;
    [SerializeField] private AudioClip[] earsRingingClips;
    [SerializeField] private AudioClip[] slipClips;
    [SerializeField] private AudioClip[] damageClips;

    [Header("Visuals Settings")]
    [SerializeField] private Material visionDistortionMaterial;
    [SerializeField] private float currentDistortionIntensity;
    [SerializeField] private float minDistortion;
    [SerializeField] private float maxDistortion;
    [SerializeField] private float currentSaturation;
    [SerializeField] private float minSaturation;
    [SerializeField] private float maxSaturation;
    [SerializeField] private float currentVignette;
    [SerializeField] private float minVignette;
    [SerializeField] private float maxVignette;
    private int DistortionProperty = Shader.PropertyToID("_Blend");
    private CinemachineVolumeSettings postProcessVolume;
    private ColorAdjustments colorAdjustments;
    private LensDistortion lensDistortion;
    private SplitToning splitToning;
    private Vignette vignette;

    [Header("Movement Settings")]
    [SerializeField] private float minMovementMultiplier = 0.5f;
    [SerializeField] private float maxMovementMultiplier = 1.5f;
    private float currentMovementMultiplier = 1f;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        GetInitComponents();
        GetVolumeEffects();
        ResetVisualEffects();
    }
    private void GetInitComponents() {
        if(cam == null) 
            cam = FindAnyObjectByType<CinemachineCamera>();
        if(playerGO == null) 
            playerGO = GameObject.FindWithTag("Player");
        if(peePS == null) 
            peePS = playerGO.GetComponentInChildren<ParticleSystem>();
    }
    private void GetVolumeEffects() {
        cam?.TryGetComponent(out postProcessVolume);
        
        postProcessVolume?.Profile.TryGet(out colorAdjustments);
        postProcessVolume?.Profile.TryGet(out lensDistortion);
        postProcessVolume?.Profile.TryGet(out splitToning);
        postProcessVolume?.Profile.TryGet(out vignette);
    }

    #region Audio Effects
    public void ApplyAllAudioSourcePitchShift(float perc) {
        var audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var source in audioSources) {
            source.pitch *= perc;
            source.pitch = Mathf.Clamp(source.pitch, 0.6f, 1.1f);
        }
    }
    #endregion

    #region Visual Effects
    public void ResetVisualEffects() {
        if (vignette != null)
            vignette.intensity.value = 0f;
        if (lensDistortion != null)
            lensDistortion.intensity.value = 0f;
        if (splitToning != null)
            splitToning.balance.value = -100f;
        if (visionDistortionMaterial != null)
            visionDistortionMaterial.SetFloat(DistortionProperty, 0.01f);
    }
    
    public void ApplySaturationAndVignette(float saturationChange, float vignetteChange, float duration) {
        if (saturationVignetteCoroutine != null) 
            StopCoroutine(saturationVignetteCoroutine);

        // Update and clamp saturation and vignette values
        currentSaturation = Mathf.Clamp(currentSaturation + saturationChange, minSaturation, maxSaturation);
        currentVignette = Mathf.Clamp(currentVignette + vignetteChange, minVignette, maxVignette);

        // Start the effect coroutine
        saturationVignetteCoroutine = StartCoroutine(SaturationVignetteCoroutine(duration));
    }
    private IEnumerator SaturationVignetteCoroutine(float duration) {
        float elapsedTime = 0f;
        float startSaturation = colorAdjustments.saturation.value;
        float startVignette = vignette.intensity.value;

        // Smoothly transition to new intensity
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, currentSaturation, t);
            vignette.intensity.value = Mathf.Lerp(startVignette, currentVignette, t);

            yield return null;
        }

        // Delay before reducing effect
        yield return new WaitForSeconds(duration * 0.5f);

        // Smoothly fade back to normal
        elapsedTime = 0f;
        float fadeStartSaturation = currentSaturation;
        float fadeStartVignette = currentVignette;
        
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            currentSaturation = Mathf.Lerp(fadeStartSaturation, 0f, t);
            currentVignette = Mathf.Lerp(fadeStartVignette, 0f, t);

            colorAdjustments.saturation.value = currentSaturation;
            vignette.intensity.value = currentVignette;

            yield return null;
        }

        // Ensure reset after full duration
        currentSaturation = Mathf.Max(0f, currentSaturation);
        currentVignette = Mathf.Max(0f, currentVignette);
    }

    public void ApplyVisionDistortion(float intensity, float duration) {
        if (visionDistortionCoroutine != null) 
            StopCoroutine(visionDistortionCoroutine);

        // Update current intensity while ensuring it stays within limits
        currentDistortionIntensity = Mathf.Clamp(currentDistortionIntensity + intensity, minDistortion, maxDistortion);

        // Start the effect coroutine
        visionDistortionCoroutine = StartCoroutine(VisionDistortionCoroutine(duration));
    }
    private IEnumerator VisionDistortionCoroutine(float duration) {
        float elapsedTime = 0f;
        float initialIntensity = visionDistortionMaterial.GetFloat(DistortionProperty);

        // Smoothly transition to the new intensity
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            visionDistortionMaterial.SetFloat(DistortionProperty, Mathf.Lerp(initialIntensity, currentDistortionIntensity, t));
            yield return null;
        }

        // Delay before reducing the effect
        yield return new WaitForSeconds(duration * 0.5f);

        // Smoothly fade the effect back down
        elapsedTime = 0f;
        float startIntensity = currentDistortionIntensity;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            currentDistortionIntensity = Mathf.Lerp(startIntensity, 0f, t);
            visionDistortionMaterial.SetFloat(DistortionProperty, currentDistortionIntensity);
            yield return null;
        }

        // Ensure reset after full duration
        currentDistortionIntensity = Mathf.Max(0f, currentDistortionIntensity);
    }
    #endregion
    
    #region Movement Effects
    public void ApplyMovementMultiplier(float multiplier, float duration) {
        if (movementMultiplierCoroutine != null) 
            StopCoroutine(movementMultiplierCoroutine);

        // Update current movement multiplier while ensuring it stays within limits
        currentMovementMultiplier = Mathf.Clamp(currentMovementMultiplier * multiplier, minMovementMultiplier, maxMovementMultiplier);

        // Apply the movement change
        Controller_Player.Instance.SetSpeedMoveAndLook(currentMovementMultiplier);

        // Start the effect coroutine
        movementMultiplierCoroutine = StartCoroutine(MovementMultiplierCoroutine(duration));
    }
    private IEnumerator MovementMultiplierCoroutine(float duration) {
        yield return new WaitForSeconds(duration);

        // Gradually return to normal speed
        float elapsedTime = 0f;
        float startMultiplier = currentMovementMultiplier;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            currentMovementMultiplier = Mathf.Lerp(startMultiplier, 1f, t);
            Controller_Player.Instance.SetSpeedMoveAndLook(currentMovementMultiplier);
            yield return null;
        }

        // Ensure reset to normal speed
        currentMovementMultiplier = 1f;
        Controller_Player.Instance.SetSpeedMoveAndLook(currentMovementMultiplier);
    }
    #endregion

    #region Kidney Stone Effects
    public void SetGotStoneEffect() {
        if (kidneyStoneEffectCoroutine != null) 
            StopCoroutine(kidneyStoneEffectCoroutine);
        
        kidneyStoneEffectCoroutine = StartCoroutine(
            StoneEffectCoroutine(
                -15f, 
                0.8f, 
                -1f
        ));
    }
    public void SetPassStoneEffect() {
        if (kidneyStonePassBoostCoroutine != null) 
            StopCoroutine(kidneyStonePassBoostCoroutine);
        
        kidneyStonePassBoostCoroutine = StartCoroutine(
            StoneEffectCoroutine(
                5f, 
                1.4f, 
                3f
            )
        );
    }
    private IEnumerator StoneEffectCoroutine(float peeAmountChange, float rateMul, float duration) {
        // var main = peePS.main;
        var emission = peePS.emission;
        var maxPeeAmount = Controller_Pee.Instance.GetMaxPeeAmount();

        // Apply changes and clamp values
        maxPeeAmount = Mathf.Clamp(maxPeeAmount + peeAmountChange, 0f, 150f);
        Controller_Pee.Instance.SetMaxPeeAmount(maxPeeAmount);
        Controller_Player.Instance.SetSpeedMoveAndLook(Mathf.Clamp(rateMul, 0.1f, 1.3f));

        // main.gravityModifier = Mathf.Clamp(main.gravityModifier.constant * rateMul, 0.2f, 2.5f);
        emission.rateOverTime = Mathf.Clamp(emission.rateOverTime.constant * rateMul, 5, 150);

        if (duration == -1f) yield break;

        yield return new WaitForSeconds(duration);

        // Revert changes after duration
        maxPeeAmount = Mathf.Clamp(maxPeeAmount - peeAmountChange, 0f, 150f);
        Controller_Pee.Instance.SetMaxPeeAmount(maxPeeAmount);
        Controller_Player.Instance.SetSpeedMoveAndLook(Mathf.Clamp(1 / rateMul, 0.1f, 1.3f));

        // main.gravityModifier = Mathf.Clamp(main.gravityModifier.constant / rateMul, 0.2f, 2.5f);
        emission.rateOverTime = Mathf.Clamp(emission.rateOverTime.constant / rateMul, 5, 150);
    }
    #endregion

    #region Dehydration Effects
    public void UpdateDehydrationEffects(float dehydration) {
        Color tempColor = Color.yellow;
        tempColor.a = Mathf.Lerp(0f, 0.5f, dehydration);
        
        var img = Manager_UI.Instance.GetDehydrationImageUI();
        img.color = tempColor;
    }
    public void ResetDehydrationEffects() {
        Color tempColor = Color.black;
        tempColor.a = 0f;

        var img = Manager_UI.Instance.GetDehydrationImageUI();
        img.color = tempColor;
    }
    #endregion

    #region Damage Effects
    public void StartDamageEffectsSeq(float time = 0.3f) {
        if (damageSequence != null) return;

        damageSequence = DOTween.Sequence()
            .OnStart(() => {
                Controller_Player.Instance.SetCanMoveAndLook(false);
                Manager_UI.Instance.GetDamageImageUI().color = Color.red;
                Helper.Instance.PlayRandAudio(audioSource, damageClips);
            })
            .Join(
                cam.transform.DOShakePosition(time, 1f, 10, 90, false, true)
            )
            .Insert(
                0f,
                Manager_UI.Instance.GetDamageImageUI().DOFade(1f, 0.05f)
            )
            .Insert(
                0f,
                Manager_UI.Instance.GetDamageImageUI().DOFade(0f, time)
            )
            .OnComplete(() => {
                StopDamageEffectsSeq();
            });
    }
    public void StopDamageEffectsSeq() {
        damageSequence.Kill();
        damageSequence = null;

        var damageImageColor = Manager_UI.Instance.GetDamageImageUI().color;
        damageImageColor.a = 0f;
        Manager_UI.Instance.GetDamageImageUI().color = damageImageColor;

        Controller_Player.Instance.SetCanMoveAndLook(true);
        audioSource.Stop();
    }
    #endregion

    #region Stun Effects
    public void StartStunEffectsSeq(float time = 1.0f) {
        if (stunSequence != null) return;

        stunSequence = DOTween.Sequence()
            .OnStart(() => {
                Controller_Player.Instance.SetCanMoveAndLook(false);
                Manager_UI.Instance.GetDamageImageUI().color = Color.red;
                Helper.Instance.PlayRandAudio(audioSource, slapClips);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() => {
                Helper.Instance.PlayRandAudio(audioSource, earsRingingClips);
            })
            .Join(
                cam.transform.DOShakePosition(time, 1f, 10, 90, false, true)
            )
            .Insert(
                0f,
                Manager_UI.Instance.GetDamageImageUI().DOFade(1f, 0.05f)
            )
            .Insert(
                0f,
                Manager_UI.Instance.GetDamageImageUI().DOFade(0f, time)
            )
            .OnComplete(() => {
                StopStunEffectsSeq();
            });
    }
    public void StopStunEffectsSeq() {
        stunSequence.Kill();
        stunSequence = null;

        var damageImageColor = Manager_UI.Instance.GetDamageImageUI().color;
        damageImageColor.a = 0f;
        Manager_UI.Instance.GetDamageImageUI().color = damageImageColor;

        Controller_Player.Instance.SetCanMoveAndLook(true);
        audioSource.Stop();
    }
    #endregion

    #region Slippery Effects
    public void StartSlipEffectsSeq(float time = 1.0f) {
        if (slipSequence != null) return;

        slipSequence = DOTween.Sequence()
            .OnStart(() => {
                Controller_Player.Instance.SetCanMoveAndLook(false);
                Helper.Instance.PlayRandAudio(audioSource, slipClips);
            })
            .Join(
                cam.transform
                    .DOShakePosition(time, 1f, 10, 90, false, true)
            )
            .Join(
                Controller_Player.Instance.transform
                    .DORotate(new Vector3(0, 360, 0), time, RotateMode.FastBeyond360)
                    .SetLoops(Mathf.CeilToInt(time / 0.9f), LoopType.Incremental)
            )
            .OnComplete(() => {
                StopSlipEffectsSeq();
            });
    }
    public void StopSlipEffectsSeq() {
        slipSequence.Kill();
        slipSequence = null;

        Controller_Player.Instance.SetCanMoveAndLook(true);
        audioSource.Stop();
    }
    #endregion
}