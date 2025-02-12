using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Linq;

public class Controller_Stain : MonoBehaviour 
{
    #region Vars
    private DecalProjector decalProjector;

    [Header("Type Settings")]
    [SerializeField] public Manager_Stains.StainType stainType;
    private Type_Stain selectedStain;
    
    [Header("Booze Properties")]
    [SerializeField] public float speedReductionMul = 0.5f;
    [SerializeField] public float speedIncreaseMul = 2f;
    
    [Header("Puke Properties")]
    [SerializeField] private float scaleAmount = 1.2f;
    [SerializeField] private float scaleTime = 5f;
    #endregion

    private void Start() {
        decalProjector = GetComponent<DecalProjector>();

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
                transform.DOScale(transform.localScale * scaleAmount, scaleTime);
                break;
        }
    }

    public void FadeOutAndDisable() {
        var fadeMul = Controller_Pee.Instance.GetFadeMul();

        decalProjector.fadeFactor -= selectedStain.fadeAmountPerCollision * fadeMul;

        if (decalProjector.fadeFactor <= 0f) {
            decalProjector.fadeFactor = 0f;
            Manager_SaveLoad.Instance.SaveStatData("totalCleanedStains", "add", 1);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other) {
        Manager_Effects.Instance.ApplyEffect(other, this);
    }
    private void OnCollisionExit(Collision other) {
        Manager_Effects.Instance.ApplyEffect(other, this);
    }
}