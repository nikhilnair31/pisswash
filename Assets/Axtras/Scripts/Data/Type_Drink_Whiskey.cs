using UnityEngine;

[CreateAssetMenu(fileName = "Whiskey Shot", menuName = "Scriptable Objects/Drinks/Whiskey", order = 3)]
public class Type_Drink_Whiskey : Type_Drink 
{
    #region Vars
    [Header("Whiskey Settings")]
    [SerializeField] private float duration = 6f;
    [SerializeField] private float speedReductionMultiplier = 10f;
    [SerializeField] private float distortionIntensity = 10f;
    [SerializeField] private float saturationIncrease = 0.1f;
    [SerializeField] private float vignetteIncrease = 0.2f;
    [SerializeField] private float pitchShiftPerc = 0.8f;
    #endregion
    
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        Debug.Log($"Type_Drink_Whiskey StartConsumptionEffect");

        // medium reduction in movement speed
        Controller_Player.Instance.SetSpeedMoveAndLook(speedReductionMultiplier);
        // medium increase in hydration
        Controller_Pee.Instance.AddPeeAmount(increaseHydrationAmount);
        // medium vision distortion
        Manager_Effects.Instance.ApplyVisionDistortion(distortionIntensity, duration);

        // medium reduction in all audio source's pitch
        Manager_Effects.Instance.ApplyAllAudioSourcePitchShift(pitchShiftPerc);
        // increased visual saturation and vignette
        Manager_Effects.Instance.ApplySaturationAndVignette(saturationIncrease, vignetteIncrease, duration);
    }
}