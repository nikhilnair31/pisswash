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
        if (Random.Range(0f, 1f) >= hydrationChangeChance)
            Controller_Pee.Instance.AddPeeAmount(RandShiftVal(incrHydrationAmt));
        // small reduction in movement speed
        if (Random.Range(0f, 1f) >= movementChangeChance)
            Manager_Effects.Instance.ApplyMovementMultiplier(speedReducMul, duration);
        // small vision distortion
        if (Random.Range(0f, 1f) >= visualsChangeChance)
            Manager_Effects.Instance.ApplyVisionDistortion(RandShiftVal(distortionAmt), RandShiftVal(duration));
        
        // random reduction in all audio source's pitch
        if (Random.Range(0f, 1f) >= audioChangeChance)
            Manager_Effects.Instance.ApplyAllAudioSourcePitchShift(pitchShiftPerc);
        // random visual values
        if (Random.Range(0f, 1f) >= visualsChangeChance)
            Manager_Effects.Instance.ApplySaturationAndVignette(saturationAmt, vignetteAmt, duration);
        
        // random chance of money increase/decrease
        if (Random.Range(0f, 1f) >= moneyChangeChance)
            Manager_Money.Instance.UpdateMoney(RandShiftVal(moneyAmt));
        // random chance of timer increase/decrease
        if (Random.Range(0f, 1f) >= timeChangeChance)
            Manager_Timer.Instance.AddTimerAmt(RandShiftVal(timeAmt));
        // random chance of slipping
        if (Random.Range(0f, 1f) >= slipChangeChance)
            Manager_Effects.Instance.StartSlipEffectsSeq(RandShiftVal(slipTimeAmt));
        // random chance of stun/damage
        if (Random.Range(0f, 1f) >= stunChangeChance)
            Manager_Effects.Instance.StartStunEffectsSeq(RandShiftVal(stunTimeAmt));
        
        // random chance of spawning more stains
        // random chance of more fountains
    }

    private float RandShiftVal(float val) {
        var percAmt = val * randPerc;
        return val + Random.Range(-percAmt, percAmt);
    }
    private int RandShiftVal(int val) {
        var percAmt = val * randPerc;
        var newVal = val + Random.Range(-percAmt, percAmt);
        return (int)newVal;
    }
}