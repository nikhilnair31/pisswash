using UnityEngine;

[CreateAssetMenu(fileName = "Mystery Drink", menuName = "Scriptable Objects/Drinks/Mystery", order = 4)]
public class Type_Drink_Mystery : Type_Drink 
{
    #region Vars
    [Header("Mystery Settings")]
    [SerializeField] private float duration = 6f;
    [SerializeField] private float randPerc = 0.2f;
    [SerializeField] private float speedReductionMultiplier = 10f;
    [SerializeField] private float distortionIntensity = 10f;
    [SerializeField] private float saturationIncrease = 0.1f;
    [SerializeField] private float vignetteIncrease = 0.2f;
    [SerializeField] private float pitchShiftPerc = 0.8f;
    #endregion
    
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        Debug.Log($"Type_Drink_Mystery StartConsumptionEffect");

        // small reduction in movement speed
        Controller_Player.Instance.SetSpeedMoveAndLook(RandShiftVal(speedReductionMultiplier));
        // small increase in hydration
        Controller_Pee.Instance.AddPeeAmount(RandShiftVal(increaseHydrationAmount));
        // small vision distortion
        Manager_Effects.Instance.ApplyVisionDistortion(RandShiftVal(distortionIntensity), RandShiftVal(duration));
        
        // random reduction in all audio source's pitch
        Manager_Effects.Instance.ApplyAllAudioSourcePitchShift(pitchShiftPerc);
        // random visual values
        Manager_Effects.Instance.ApplySaturationAndVignette(saturationIncrease, vignetteIncrease, duration);
        
        // random chance of money increase/decrease
        // random chance of slipping
        // random chance of stun/damage
        // random chance of spawning more stains
        // random chance of more fountains
        // random chance of timer increase/decrease
    }

    private float RandShiftVal(float val) {
        var percAmt = val * randPerc;
        return val + Random.Range(-percAmt, percAmt);
    }
}