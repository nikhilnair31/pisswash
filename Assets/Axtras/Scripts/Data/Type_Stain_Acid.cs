using UnityEngine;

[CreateAssetMenu(fileName = "Acid", menuName = "Scriptable Objects/Stains/Acid", order = 3)]
public class Type_Stain_Acid : Type_Stain 
{
    #region Vars
    [Header("Acid Settings")]
    [SerializeField] public float damageTime = 1f;
    #endregion
    
    public override void StartInteractionEffect() {
        base.StartInteractionEffect();

        Manager_Effects.Instance.StartDamageEffectsSeq(damageTime);
    }
}