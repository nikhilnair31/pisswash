using UnityEngine;

[CreateAssetMenu(fileName = "Whiskey Shot", menuName = "Scriptable Objects/Drinks/Whiskey", order = 3)]
public class Type_Drink_Whiskey : Type_Drink 
{
    #region Vars
    [Header("Whiskey Settings")]
    [SerializeField] private float duration = 6f;
    [SerializeField] private float hydrationAmount = 10f;
    [SerializeField] private float speedReductionMultiplier = 10f;
    [SerializeField] private float distortionIntensity = 10f;
    #endregion
    
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        Debug.Log($"Type_Drink_Whiskey StartConsumptionEffect");

        // small reduction in movement speed
        Controller_Player.Instance.SetSpeedMoveAndLook(speedReductionMultiplier);
        // small increase in hydration
        Controller_Pee.Instance.AddPeeAmount(hydrationAmount);
        // small vision distortion
        Manager_Effects.Instance.ApplyVisionDistortion(distortionIntensity, duration);

        // medium reduction in all audio source's pitch
        // increased visual saturation and vignette
    }
}