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
    private Coroutine kidneyStoneEffectCoroutine;
    private Coroutine kidneyStonePassBoostCoroutine;

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
    [SerializeField] private float maxYellowAlpha = 0.1f;
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
    
    #region Init
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
        if(peePS == null && playerGO != null) 
            peePS = playerGO.GetComponentInChildren<ParticleSystem>();
    }
    private void GetVolumeEffects() {
        cam?.TryGetComponent(out postProcessVolume);
        
        postProcessVolume?.Profile.TryGet(out colorAdjustments);
        postProcessVolume?.Profile.TryGet(out lensDistortion);
        postProcessVolume?.Profile.TryGet(out splitToning);
        postProcessVolume?.Profile.TryGet(out vignette);
    }
    #endregion

    #region Audio Effects
    public void ApplyAllAudioSourcePitchShift(float mul, float duration) {
        var audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        DOTween.Kill("AudioPitchShift");
        var sequence = DOTween.Sequence().SetId("AudioPitchShift");
        foreach (var source in audioSources) {
            float targetPitch = Mathf.Clamp(source.pitch * mul, 0.6f, 1.1f);
            sequence.Join(
                DOTween.To(() => source.pitch, x => source.pitch = x, targetPitch, duration)
            );
        }
        sequence.AppendInterval(duration * 0.5f);
        foreach (var source in audioSources) {
            sequence.Join(
                DOTween.To(() => source.pitch, x => source.pitch = x, 1f, duration)
            );
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
        if (colorAdjustments != null)
            colorAdjustments.saturation.value = 0f;
        if (visionDistortionMaterial != null)
            visionDistortionMaterial.SetFloat(DistortionProperty, 0.01f);
    }
    
    public void ApplySaturationAndVignette(float saturationMul, float vignetteMul, float duration) {
        currentSaturation = Mathf.Clamp(currentSaturation * saturationMul, minSaturation, maxSaturation);
        currentVignette = Mathf.Clamp(currentVignette * vignetteMul, minVignette, maxVignette);

        DOTween.Kill("SaturationVignette");
        DOTween.Sequence().SetId("SaturationVignette")
            .Append(
                DOTween.To(() => colorAdjustments.saturation.value, x => colorAdjustments.saturation.value = x, currentSaturation, duration)
            )
            .Join(
                DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, currentVignette, duration)
            )
            .AppendInterval(duration * 0.5f)
            .Append(
                DOTween.To(() => colorAdjustments.saturation.value, x => colorAdjustments.saturation.value = x, 0f, duration)
            )
            .Join(
                DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0f, duration)
            );
    }

    public void ApplyVisionDistortion(float mul, float duration) {
        currentDistortionIntensity = Mathf.Clamp(currentDistortionIntensity * mul, minDistortion, maxDistortion);

        DOTween.Kill("VisionDistortion");
        DOTween.Sequence().SetId("VisionDistortion")
            .Append(
                DOTween.To(() => visionDistortionMaterial.GetFloat(DistortionProperty), x => visionDistortionMaterial.SetFloat(DistortionProperty, x), currentDistortionIntensity, duration)
            )
            .AppendInterval(duration * 0.5f)
            .Append(
                DOTween.To(() => visionDistortionMaterial.GetFloat(DistortionProperty), x => visionDistortionMaterial.SetFloat(DistortionProperty, x), 0.01f, duration)
            );
    }
    #endregion
    
    #region Movement Effects
    public void ApplyMovementMultiplier(float mul, float duration) {
        currentMovementMultiplier = Mathf.Clamp(currentMovementMultiplier * mul, minMovementMultiplier, maxMovementMultiplier);
        Controller_Player.Instance.SetSpeedMoveAndLook(currentMovementMultiplier);

        DOTween.Kill("MovementMultiplier");
        DOTween.Sequence().SetId("MovementMultiplier")
            .AppendInterval(duration)
            .Append(
                DOTween.To(() => currentMovementMultiplier, 
                x => {
                    currentMovementMultiplier = x;
                    Controller_Player.Instance.SetSpeedMoveAndLook(x);
                }, 1f, duration)
            );
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
        tempColor.a = Mathf.Lerp(0f, maxYellowAlpha, dehydration);
        
        var img = Manager_UI.Instance.GetDehydrationImageUI();
        img.color = tempColor;
    }
    public void ResetDehydrationEffects() {
        Color tempColor = Color.black;
        tempColor.a = 0f;

        var img = Manager_UI.Instance.GetDehydrationImageUI();
        img.color = tempColor;
    }
    
    public void ShakeDehydrationEffect(float intensity) {
        var uiElement = Manager_UI.Instance.GetPeeRectTransfUI();

        // uiElement.DOComplete();

        uiElement
            .DOShakePosition(0.05f, intensity * 10f, 20, 90, false, true);
    }
    public void ExplodeDehydrationEffect() {
        var uiElement = Manager_UI.Instance.GetPeeRectTransfUI();

        uiElement.DOComplete();
        
        uiElement
            .DOScale(Vector3.one * 2f, 0.2f)
            .OnComplete(() => {
                uiElement
                    .DOScale(Vector3.one, 0.4f);
            });
    }
    #endregion

    #region Damage Effects
    public void StartDamageEffectsSeq(float time = 0.3f) {
        if (damageSequence != null) return;

        damageSequence = DOTween.Sequence()
            .OnStart(() => {
                Controller_Player.Instance.SetCanMoveAndLook(false);
                Manager_UI.Instance.GetDamageImageUI().color = Color.red;
                Manager_Audio.Instance.PlayRandAudio(audioSource, damageClips);
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
                Manager_Audio.Instance.PlayRandAudio(audioSource, slapClips);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() => {
                Manager_Audio.Instance.PlayRandAudio(audioSource, earsRingingClips);
            })
            .Join(
                cam?.transform.DOShakePosition(time, 1f, 10, 90, false, true)
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
                Manager_Audio.Instance.PlayRandAudio(audioSource, slipClips);
            })
            .Join(
                cam?.transform
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