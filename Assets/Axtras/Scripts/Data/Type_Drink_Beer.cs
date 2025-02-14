using UnityEngine;

[CreateAssetMenu(fileName = "Beer Bottle", menuName = "Scriptable Objects/Drinks/Beer", order = 2)]
public class Type_Drink_Beer : Type_Drink 
{
    #region Vars
    [Header("Beer Settings")]
    [SerializeField] private float duration = 6f;
    [SerializeField] private float speedReductionMultiplier = 10f;
    [SerializeField] private float distortionIntensity = 10f;
    #endregion
    
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        Debug.Log($"Type_Drink_Beer StartConsumptionEffect");

        // small reduction in movement speed
        Controller_Player.Instance.SetSpeedMoveAndLook(speedReductionMultiplier);
        // small increase in hydration
        Controller_Pee.Instance.AddPeeAmount(increaseHydrationAmount);
        // small vision distortion
        Manager_Effects.Instance.ApplyVisionDistortion(distortionIntensity, duration);
    }
}