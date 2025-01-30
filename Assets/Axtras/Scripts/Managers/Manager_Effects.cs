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

    [Header("Effect Settings")]
    [SerializeField] private CinemachineVolumeSettings postProcessVolume;
    private Vignette vignette;
    private LensDistortion lensDistortion;
    private SplitToning splitToning;

    [Header("Mouse Settings")]
    [SerializeField] private float slapShakeDuration = 2f;
    [SerializeField] private float slapDelayDuration = 0f;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        postProcessVolume = cam.GetComponent<CinemachineVolumeSettings>();
        
        GetVolumeEffects();
    }  
    private void GetVolumeEffects() {
        if (postProcessVolume.Profile.TryGet(out vignette)) 
            vignette.intensity.value = 0f;
        if (postProcessVolume.Profile.TryGet(out lensDistortion)) 
            lensDistortion.intensity.value = 0f;
        if (postProcessVolume.Profile.TryGet(out splitToning)) 
            splitToning.balance.value = -100f;
    }

    public void UpdateDehydrationEffects(float dehydration) {
        if (vignette != null) 
            vignette.intensity.value = Mathf.Lerp(0f, 0.5f, dehydration);
        
        if (lensDistortion != null) 
            lensDistortion.intensity.value = Mathf.Lerp(0f, 0.5f, dehydration);
        
        if (splitToning != null) 
            splitToning.balance.value = Mathf.Lerp(-100f, 100f, dehydration);
    }
    public void ResetDehydratedEffects() {
        if (vignette != null) 
            vignette.intensity.value = 0f;
        if (lensDistortion != null) 
            lensDistortion.intensity.value = 0f;
        if (splitToning != null) 
            splitToning.balance.value = -100f;
    }
    
    public void GotSlapped() {
        DOTween.Sequence()
            .OnStart(() => {
                Controller_Player.Instance.ControlCanMoveAndLook(false);
            })
            .Append(
                cam.transform.DOShakePosition(slapShakeDuration, 1f, 10, 90, false, true)
            )
            .AppendInterval(
                slapDelayDuration
            )
            .OnComplete(() => {
                Controller_Player.Instance.ControlCanMoveAndLook(true);
            });
    }
}