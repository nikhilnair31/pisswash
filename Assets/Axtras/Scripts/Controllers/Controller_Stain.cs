using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Controller_Stain : MonoBehaviour 
{
    public enum StainType { Acid, Booze, Puke }

    #region Vars
    [Header("Control Properties")]
    [SerializeField] public StainType stainType;
    [SerializeField] private float fadeAmountPerCollision = 0.01f;
    private DecalProjector decalProjector;
    
    [Header("Acid Properties")]
    
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
        switch (stainType) {
            case StainType.Acid:
                break;
            case StainType.Booze:
                break;
            case StainType.Puke:
                transform.DOScale(transform.localScale * scaleAmount, scaleTime);
                break;
        }
    }

    public void FadeOutAndDisable() {
        var fadeMul = Controller_Pee.Instance.GetFadeMul();

        decalProjector.fadeFactor -= fadeAmountPerCollision * fadeMul;

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