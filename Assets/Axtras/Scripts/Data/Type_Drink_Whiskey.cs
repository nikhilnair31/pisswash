using UnityEngine;

[CreateAssetMenu(fileName = "Whiskey Shot", menuName = "Scriptable Objects/Drinks/Whiskey", order = 3)]
public class Type_Drink_Whiskey : Type_Drink 
{
    #region Vars
    [Header("Effect Settings")]
    [SerializeField] private float duration = 8f;

    [Header("Movement Settings")]
    [SerializeField] private float speedReducMul = 0.8f;
    
    [Header("Visuals Settings")]
    [SerializeField] private float distortionMul = 1.2f;
    [SerializeField] private float saturationMul = 1.2f;
    [SerializeField] private float vignetteMul = 1.2f;
    
    [Header("Audio Settings")]
    [SerializeField] private float pitchShiftPerc = 0.8f;
    #endregion
    
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        // medium increase in hydration
        Controller_Pee.Instance.AddPeeAmount(incrHydrationAmt);

        // medium reduction in movement speed
        Manager_Effects.Instance.ApplyMovementMultiplier(speedReducMul, duration);
        // medium vision distortion
        Manager_Effects.Instance.ApplyVisionDistortion(distortionMul, duration);

        // medium reduction in all audio source's pitch
        Manager_Effects.Instance.ApplyAllAudioSourcePitchShift(pitchShiftPerc, duration);
        // increased visual saturation and vignette
        Manager_Effects.Instance.ApplySaturationAndVignette(saturationMul, vignetteMul, duration);
    }
}