using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Manager_Effects : MonoBehaviour 
{
    #region Vars
    public static Manager_Effects Instance { get; private set; }

    [Header("General Settings")]
    [SerializeField] private CinemachineCamera cam;
    private CinemachineVolumeSettings postProcessVolume;
    private Vignette vignette;
    private LensDistortion lensDistortion;
    private SplitToning splitToning;

    [Header("Mouse Settings")]
    [SerializeField] private float slapShakeDuration = 2f;
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
        
        var img = Manager_UI.Instance.GetEffectsImageUI();
        img.color = tempColor;
    }

    public void ImpactEffects() {
        DOTween.Sequence()
            .OnStart(() => {
                Controller_Player.Instance.ControlCanMoveAndLook(false);
                Manager_UI.Instance.GetEffectsImageUI().color = Color.red;
            })
            .Join(
                cam.transform.DOShakePosition(slapShakeDuration, 1f, 10, 90, false, true)
            )
            .Insert(
                0f,
                Manager_UI.Instance.GetEffectsImageUI().DOFade(1f, 0.05f)
            )
            .Insert(
                0f,
                Manager_UI.Instance.GetEffectsImageUI().DOFade(0f, slapShakeDuration)
            )
            .OnComplete(() => {
                Controller_Player.Instance.ControlCanMoveAndLook(true);
            });
    }
}