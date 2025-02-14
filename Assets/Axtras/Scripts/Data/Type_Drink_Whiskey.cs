using UnityEngine;

[CreateAssetMenu(fileName = "Whiskey Shot", menuName = "Scriptable Objects/Drinks/Whiskey", order = 3)]
public class Type_Drink_Whiskey : Type_Drink 
{
    #region Vars
    [Header("Effect Settings")]
    [SerializeField] private float duration = 6f;

    [Header("Movement Settings")]
    [SerializeField] private float speedReducMul = 10f;
    
    [Header("Visuals Settings")]
    [SerializeField] private float distortionAmt = 0.01f;
    [SerializeField] private float saturationAmt = 0.1f;
    [SerializeField] private float vignetteAmt = 0.2f;
    
    [Header("Audio Settings")]
    [SerializeField] private float pitchShiftPerc = 0.8f;
    #endregion
    
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        // Debug.Log($"Type_Drink_Whiskey StartConsumptionEffect");

        // medium increase in hydration
        Controller_Pee.Instance.AddPeeAmount(incrHydrationAmt);
        // medium reduction in movement speed
        Manager_Effects.Instance.ApplyMovementMultiplier(speedReducMul, duration);
        // medium vision distortion
        Manager_Effects.Instance.ApplyVisionDistortion(distortionAmt, duration);

        // medium reduction in all audio source's pitch
        Manager_Effects.Instance.ApplyAllAudioSourcePitchShift(pitchShiftPerc);
        // increased visual saturation and vignette
        Manager_Effects.Instance.ApplySaturationAndVignette(saturationAmt, vignetteAmt, duration);
    }
}