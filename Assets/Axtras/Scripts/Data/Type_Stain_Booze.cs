using UnityEngine;

[CreateAssetMenu(fileName = "Booze", menuName = "Scriptable Objects/Stains/Booze", order = 2)]
public class Type_Stain_Booze : Type_Stain 
{
    #region Vars
    [Header("Booze Settings")]
    [SerializeField] public float speedReducMul = 0.8f;
    [SerializeField] public float speedReducTime = 1f;
    #endregion
    
    public override void StartInteractionEffect() {
        base.StartInteractionEffect();
        
        Manager_Effects.Instance.ApplyMovementMultiplier(speedReducMul, speedReducTime);
    }
}