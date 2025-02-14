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
    [SerializeField] private float speedReducMul = 10f;
    
    [Header("Visuals Settings")]
    [SerializeField] private float visualsChangeChance = 0.5f;
    [SerializeField] private float distortionAmt = 0.01f;
    [SerializeField] private float saturationAmt = 0.1f;
    [SerializeField] private float vignetteAmt = 0.2f;
    
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
                Helper.Instance.RandShiftVal(incrHydrationAmt, randPerc)
            );
        // small reduction in movement speed
        if (Helper.Instance.TriggerBool(movementChangeChance))
            Manager_Effects.Instance.ApplyMovementMultiplier(
                Helper.Instance.RandShiftVal(speedReducMul, randPerc), 
                Helper.Instance.RandShiftVal(duration, randPerc)
            );
        // small vision distortion
        if (Helper.Instance.TriggerBool(visualsChangeChance))
            Manager_Effects.Instance.ApplyVisionDistortion(
                Helper.Instance.RandShiftVal(distortionAmt, randPerc), 
                Helper.Instance.RandShiftVal(duration, randPerc)
            );
        
        // random reduction in all audio source's pitch
        if (Helper.Instance.TriggerBool(audioChangeChance))
            Manager_Effects.Instance.ApplyAllAudioSourcePitchShift(
                Helper.Instance.RandShiftVal(pitchShiftPerc, randPerc)
            );
        // random visual values
        if (Helper.Instance.TriggerBool(visualsChangeChance))
            Manager_Effects.Instance.ApplySaturationAndVignette(
                Helper.Instance.RandShiftVal(saturationAmt, randPerc),
                Helper.Instance.RandShiftVal(vignetteAmt, randPerc),
                Helper.Instance.RandShiftVal(duration, randPerc)
            );
        
        // random chance of money increase/decrease
        if (Helper.Instance.TriggerBool(moneyChangeChance))
            Manager_Money.Instance.UpdateMoney(
                Helper.Instance.RandShiftVal(moneyAmt, randPerc)
            );
        // random chance of timer increase/decrease
        if (Helper.Instance.TriggerBool(timeChangeChance))
            Manager_Timer.Instance.AddTimerAmt(
                Helper.Instance.RandShiftVal(timeAmt, randPerc)
            );
        // random chance of slipping
        if (Helper.Instance.TriggerBool(slipChangeChance))
            Manager_Effects.Instance.StartSlipEffectsSeq(
                Helper.Instance.RandShiftVal(slipTimeAmt, randPerc)
            );
        // random chance of stun/damage
        if (Helper.Instance.TriggerBool(stunChangeChance))
            Manager_Effects.Instance.StartStunEffectsSeq(
                Helper.Instance.RandShiftVal(stunTimeAmt, randPerc)
            );
        
        // random chance of spawning more stains
        // random chance of more fountains
    }
}