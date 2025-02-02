using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Controller_Stain : MonoBehaviour 
{
    public enum StainType { Acid, Booze, Puke }

    #region Vars
    [Header("Control Properties")]
    [SerializeField] private StainType stainType;
    [SerializeField] private float fadeAmountPerCollision = 0.01f;
    private DecalProjector decalProjector;
    
    [Header("Acid Properties")]
    
    [Header("Booze Properties")]
    [SerializeField] private float speedReductionMul = 0.5f;
    [SerializeField] private float speedIncreaseMul = 2f;
    
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
        decalProjector.fadeFactor -= fadeAmountPerCollision;

        if (decalProjector.fadeFactor <= 0f) {
            decalProjector.fadeFactor = 0f;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        ApplyEffect(other, speedReductionMul);
    }
    private void OnTriggerExit(Collider other) {
        ApplyEffect(other, speedIncreaseMul);
    }
    private void ApplyEffect(Collider other, float speedMul) {
        if (other.CompareTag("Player")) {
            switch (stainType) {
                case StainType.Acid:
                    Manager_Hazards.Instance.AddDamage();
                    break;
                case StainType.Booze:
                    Controller_Player.Instance.ControlSpeedMoveAndLook(speedMul);
                    break;
                case StainType.Puke:
                    break;
            }
        }
    }
}