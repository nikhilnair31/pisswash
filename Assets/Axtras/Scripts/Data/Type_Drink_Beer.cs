using UnityEngine;

[CreateAssetMenu(fileName = "Beer Bottle", menuName = "Scriptable Objects/Drinks/Beer", order = 2)]
public class Type_Drink_Beer : Type_Drink 
{
    #region Vars
    [Header("Effect Settings")]
    [SerializeField] private float duration = 6f;

    [Header("Movement Settings")]
    [SerializeField] private float speedReducMul = 0.9f;
    
    [Header("Visuals Settings")]
    [SerializeField] private float distortionAmt = 0.01f;
    #endregion
    
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        // small increase in hydration
        Controller_Pee.Instance.AddPeeAmount(incrHydrationAmt);
        // small reduction in movement speed
        Manager_Effects.Instance.ApplyMovementMultiplier(speedReducMul, duration);
        // small vision distortion
        Manager_Effects.Instance.ApplyVisionDistortion(distortionAmt, duration);
    }
}