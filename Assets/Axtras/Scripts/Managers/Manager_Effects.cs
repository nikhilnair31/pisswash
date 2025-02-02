using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections;

public class Manager_Effects : MonoBehaviour 
{
    #region Vars
    public static Manager_Effects Instance { get; private set; }

    [Header("Mouse Settings")]
    [SerializeField] private float slapShakeDuration = 2f;

    [Header("Visuals Settings")]
    [SerializeField] private CinemachineCamera cam;
    private CinemachineVolumeSettings postProcessVolume;
    private Vignette vignette;
    private LensDistortion lensDistortion;
    private SplitToning splitToning;

    [Header("Pee Settings")]
    [SerializeField] private ParticleSystem peePS;

    [Header("Kidney Stone Settings")]
    private Coroutine kidneyStoneEffectCoroutine;
    private Coroutine kidneyStonePassBoostCoroutine;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        GetVolumeEffects();
        ResetGameOverEffects();
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

    public void ApplyEffect(Collider other, Controller_Stain stain) {
        if (other.CompareTag("Player")) {
            switch (stain.stainType) {
                case Controller_Stain.StainType.Acid:
                    Manager_Hazards.Instance.AddDamage();
                    break;
                case Controller_Stain.StainType.Booze:
                    Controller_Player.Instance.SetSpeedMoveAndLook(stain.speedReductionMul);
                    break;
                case Controller_Stain.StainType.Puke:
                    break;
            }
        }
    }

    public void UpdateGameOverEffects(float over) {
        if (vignette != null) 
            vignette.intensity.value = Mathf.Lerp(0f, 0.5f, over);
        if (lensDistortion != null) 
            lensDistortion.intensity.value = Mathf.Lerp(0f, 0.5f, over);
        if (splitToning != null) 
            splitToning.balance.value = Mathf.Lerp(-100f, 100f, over);
    }
    public void ResetGameOverEffects() {
        if (vignette != null)
            vignette.intensity.value = 0f;
        if (lensDistortion != null)
            lensDistortion.intensity.value = 0f;
        if (splitToning != null)
            splitToning.balance.value = -100f;
    }

    public void DehydrationEffects(float dehydration) {
        Color tempColor = Color.yellow;
        tempColor.a = Mathf.Lerp(0f, 0.5f, dehydration);
        
        var img = Manager_UI.Instance.GetDehydrationImageUI();
        img.color = tempColor;
    }

    public void DamageEffects() {
        DOTween.Sequence()
            .OnStart(() => {
                Controller_Player.Instance.SetCanMoveAndLook(false);
                Manager_UI.Instance.GetDamageImageUI().color = Color.red;
            })
            .Join(
                cam.transform.DOShakePosition(slapShakeDuration, 1f, 10, 90, false, true)
            )
            .Insert(
                0f,
                Manager_UI.Instance.GetDamageImageUI().DOFade(1f, 0.05f)
            )
            .Insert(
                0f,
                Manager_UI.Instance.GetDamageImageUI().DOFade(0f, slapShakeDuration)
            )
            .OnComplete(() => {
                Controller_Player.Instance.SetCanMoveAndLook(true);
            });
    }
}