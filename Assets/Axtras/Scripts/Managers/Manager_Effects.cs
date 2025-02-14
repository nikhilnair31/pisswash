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
    private CinemachineVolumeSettings postProcessVolume;
    private Vignette vignette;
    private LensDistortion lensDistortion;
    private SplitToning splitToning;
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

    [Header("Stun Settings")]
    [SerializeField] private float shakeTime = 2f;
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
        ResetLevelOverEffects();
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
        
        postProcessVolume?.Profile.TryGet(out vignette);
        postProcessVolume?.Profile.TryGet(out lensDistortion);
        postProcessVolume?.Profile.TryGet(out splitToning);
    }

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

    public void ApplyEffect(Collision other, Controller_Stain stain) {
        if (other.transform.CompareTag("Player")) {
            switch (stain.stainType) {
                case Manager_Stains.StainType.Acid:
                    StartDamageEffectsSeq();
                    break;
                case Manager_Stains.StainType.Booze:
                    Controller_Player.Instance.SetSpeedMoveAndLook(stain.speedReductionMul);
                    break;
                case Manager_Stains.StainType.Puke:
                    break;
            }
        }
    }

    public void UpdateLevelOverEffects(float over) {
        if (vignette != null) 
            vignette.intensity.value = Mathf.Lerp(0f, 0.5f, over);
        if (lensDistortion != null) 
            lensDistortion.intensity.value = Mathf.Lerp(0f, 0.5f, over);
        if (splitToning != null) 
            splitToning.balance.value = Mathf.Lerp(-100f, 100f, over);
    }
    public void ResetLevelOverEffects() {
        if (vignette != null)
            vignette.intensity.value = 0f;
        if (lensDistortion != null)
            lensDistortion.intensity.value = 0f;
        if (splitToning != null)
            splitToning.balance.value = -100f;
    }

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
    
    public void StartStunEffectsSeq() {
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
                cam.transform.DOShakePosition(shakeTime, 1f, 10, 90, false, true)
            )
            .Insert(
                0f,
                Manager_UI.Instance.GetDamageImageUI().DOFade(1f, 0.05f)
            )
            .Insert(
                0f,
                Manager_UI.Instance.GetDamageImageUI().DOFade(0f, shakeTime)
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
    
    public void StartSlipEffectsSeq(float time = 1f) {
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
}