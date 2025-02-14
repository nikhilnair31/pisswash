using UnityEngine;

[CreateAssetMenu(fileName = "Booze", menuName = "Scriptable Objects/Stains/Booze", order = 2)]
public class Type_Stain_Booze : Type_Stain 
{
    #region Vars
    [Header("Booze Settings")]
    [SerializeField] public float speedReductionMul = 0.8f;
    #endregion
    
    public override void StartInteractionEffect() {
        base.StartInteractionEffect();
        
        Debug.Log($"Booze StartInteractionEffect");
        
        Controller_Player.Instance.SetSpeedMoveAndLook(speedReductionMul);
    }
}