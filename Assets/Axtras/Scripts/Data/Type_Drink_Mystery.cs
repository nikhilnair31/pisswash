using UnityEngine;

[CreateAssetMenu(fileName = "Mystery Drink", menuName = "Scriptable Objects/Drinks/Mystery", order = 4)]
public class Type_Drink_Mystery : Type_Drink 
{
    #region Vars
    [Header("Effect Settings")]
    [SerializeField] private float duration = 6f;
    [SerializeField] private float randPerc = 0.2f;

    [Header("Hydration Settings")]
    [SerializeField] private float hydrationChangeChance = 0.5f;

    [Header("Movement Settings")]
    [SerializeField] private float movementChangeChance = 0.5f;
    [SerializeField] private float speedReducMul = 0.7f;
    
    [Header("Visuals Settings")]
    [SerializeField] private float visualsChangeChance = 0.5f;
    [SerializeField] private float distortionMul = 1.3f;
    [SerializeField] private float saturationMul = 1.3f;
    [SerializeField] private float vignetteMul = 1.3f;
    
    [Header("Audio Settings")]
    [SerializeField] private float audioChangeChance = 0.5f;
    [SerializeField] private float pitchShiftPerc = 0.8f;
    
    [Header("Money Settings")]
    [SerializeField] private float moneyChangeChance = 0.5f;
    [SerializeField] private int moneyAmt = 10;
    
    [Header("Time Settings")]
    [SerializeField] private float timeChangeChance = 0.5f;
    [SerializeField] private float timeAmt = 10f;
    
    [Header("Slippery Settings")]
    [SerializeField] private float slipChangeChance = 0.5f;
    [SerializeField] private float slipTimeAmt = 10f;
    
    [Header("Stun Settings")]
    [SerializeField] private float stunChangeChance = 0.5f;
    [SerializeField] private float stunTimeAmt = 10f;
    #endregion
    
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();

        // small increase in hydration
        if (Helper.Instance.TriggerBool(hydrationChangeChance))
            Controller_Pee.Instance.AddPeeAmount(
                Helper.Instance.RandShiftVal(incrHydrationAmt)
            );
        // small reduction in movement speed
        if (Helper.Instance.TriggerBool(movementChangeChance))
            Manager_Effects.Instance.ApplyMovementMultiplier(
                Helper.Instance.RandShiftVal(speedReducMul), 
                Helper.Instance.RandShiftVal(duration)
            );
        // small vision distortion
        if (Helper.Instance.TriggerBool(visualsChangeChance))
            Manager_Effects.Instance.ApplyVisionDistortion(
                Helper.Instance.RandShiftVal(distortionMul), 
                Helper.Instance.RandShiftVal(duration)
            );
        
        // random reduction in all audio source's pitch
        if (Helper.Instance.TriggerBool(audioChangeChance))
            Manager_Effects.Instance.ApplyAllAudioSourcePitchShift(
                Helper.Instance.RandShiftVal(pitchShiftPerc), 
                Helper.Instance.RandShiftVal(duration)
            );
        // random visual values
        if (Helper.Instance.TriggerBool(visualsChangeChance))
            Manager_Effects.Instance.ApplySaturationAndVignette(
                Helper.Instance.RandShiftVal(saturationMul),
                Helper.Instance.RandShiftVal(vignetteMul),
                Helper.Instance.RandShiftVal(duration)
            );
        
        // random chance of money increase/decrease
        if (Helper.Instance.TriggerBool(moneyChangeChance))
            Manager_Money.Instance.UpdateMoney(
                Helper.Instance.RandShiftVal(moneyAmt)
            );
        // random chance of timer increase/decrease
        if (Helper.Instance.TriggerBool(timeChangeChance))
            Manager_Timer.Instance.AddTimerAmt(
                Helper.Instance.RandShiftVal(timeAmt)
            );
        // random chance of slipping
        if (Helper.Instance.TriggerBool(slipChangeChance))
            Manager_Effects.Instance.StartSlipEffectsSeq(
                Helper.Instance.RandShiftVal(slipTimeAmt)
            );
        // random chance of stun/damage
        if (Helper.Instance.TriggerBool(stunChangeChance))
            Manager_Effects.Instance.StartStunEffectsSeq(
                Helper.Instance.RandShiftVal(stunTimeAmt)
            );
        
        // random chance of spawning more stains
        // random chance of more fountains
    }
}