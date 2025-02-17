using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Linq;

public class Controller_Stain : MonoBehaviour 
{
    #region Vars
    private DecalProjector decalProjector;
    private Collider boxCollider;

    [Header("Type Settings")]
    [SerializeField] public Manager_Stains.StainType stainType;
    private Type_Stain selectedStain;
    
    [Header("Audio Properties")]
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private AudioClip[] cleanedClips;
    #endregion

    private void Start() {
        decalProjector = GetComponent<DecalProjector>();
        boxCollider = GetComponent<Collider>();

        InitEffect();
    }
    private void InitEffect() {
        selectedStain = null;
        switch (stainType) {
            case Manager_Stains.StainType.Acid:
                selectedStain = Manager_Stains.Instance.stainTypesSO.FirstOrDefault(tb => tb.type == Manager_Stains.StainType.Acid);
                break;
            case Manager_Stains.StainType.Booze:
                selectedStain = Manager_Stains.Instance.stainTypesSO.FirstOrDefault(tb => tb.type == Manager_Stains.StainType.Booze);
                break;
            case Manager_Stains.StainType.Puke:
                selectedStain = Manager_Stains.Instance.stainTypesSO.FirstOrDefault(tb => tb.type == Manager_Stains.StainType.Puke);
                // transform.DOScale(transform.localScale * scaleAmount, scaleTime);
                break;
        }
    }

    public void FadeOutAndDisable() {
        var fadeMul = Controller_Pee.Instance.GetFadeMul();

        decalProjector.fadeFactor -= selectedStain.fadeAmountPerCollision * fadeMul;

        if (decalProjector.fadeFactor <= 0f) {
            decalProjector.fadeFactor = 0f;
            Helper.Instance.PlayRandAudio(audioSource, cleanedClips);
            Manager_SaveLoad.Instance.SaveStatData("totalCleanedStains", "add", 1);
            decalProjector.enabled = false;
            boxCollider.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision other) {
        StainEffect();
    }
    private void OnCollisionExit(Collision other) {
        StainEffect();
    }
    private void StainEffect() {
        selectedStain.StartInteractionEffect();
    }
}