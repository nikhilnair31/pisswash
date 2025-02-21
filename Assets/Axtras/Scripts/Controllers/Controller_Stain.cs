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
    
    [Header("Money Properties")]
    [SerializeField] public int moneyGainedOnClean = 3;
    
    [Header("Audio Properties")]
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
            case Manager_Stains.StainType.Puke:
                selectedStain = Manager_Stains.Instance.stainTypesSO.FirstOrDefault(tb => tb.type == Manager_Stains.StainType.Puke);
                if (selectedStain is Type_Stain_Puke pukeStain) {
                    transform.DOScale(transform.localScale * pukeStain.scaleAmount, pukeStain.scaleTime);
                }
                break;
            case Manager_Stains.StainType.Booze:
                selectedStain = Manager_Stains.Instance.stainTypesSO.FirstOrDefault(tb => tb.type == Manager_Stains.StainType.Booze);
                if (selectedStain is Type_Stain_Booze boozeStain) {
                    // Something
                }
                break;
            case Manager_Stains.StainType.Acid:
                selectedStain = Manager_Stains.Instance.stainTypesSO.FirstOrDefault(tb => tb.type == Manager_Stains.StainType.Acid);
                if (selectedStain is Type_Stain_Acid acidStain) {
                    // Something
                }
                break;
        }
    }

    public void FadeOutAndDisable() {
        var fadeMul = Controller_Pee.Instance.GetFadeMul();

        decalProjector.fadeFactor -= selectedStain.fadeAmountPerCollision * fadeMul;

        if (decalProjector.fadeFactor <= 0.2f) {
            // Make fully transparent
            decalProjector.fadeFactor = 0f;
            // Play cleaned stain audio clip
            Manager_Audio.Instance.PlayAudioStain(cleanedClips);
            // Give me money
            Manager_Money.Instance.UpdateMoney(moneyGainedOnClean);
            // Save to data
            Manager_SaveLoad.Instance.SaveStatData("totalCleanedStains", "add", 1);
            // Disable decal and collider
            decalProjector.enabled = false;
            boxCollider.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (Helper.Instance.GetIsPlayer(other)) {
            StainEffect();
        }
    }
    private void OnCollisionExit(Collision other) {
        // StainEffect();
    }
    private void StainEffect() {
        Debug.Log($"StainEffect started from {transform.parent.name}");

        selectedStain.StartInteractionEffect();
    }
}