using UnityEngine;

[CreateAssetMenu(fileName = "Acid", menuName = "Scriptable Objects/Stains/Acid", order = 3)]
public class Type_Stain_Acid : Type_Stain 
{
    #region Vars
    #endregion
    
    public override void StartInteractionEffect() {
        base.StartInteractionEffect();
        
        Debug.Log($"Acid StartInteractionEffect");

        Manager_Effects.Instance.StartDamageEffectsSeq();
    }
}