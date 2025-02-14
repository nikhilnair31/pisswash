using UnityEngine;

[CreateAssetMenu(fileName = "Mystery Drink", menuName = "Scriptable Objects/Drinks/Mystery", order = 4)]
public class Type_Drink_Mystery : Type_Drink 
{
    #region Vars
    [Header("Mystery Settings")]
    [SerializeField] private float duration = 6f;
    [SerializeField] private float randPerc = 0.2f;
    [SerializeField] private float hydrationAmount = 10f;
    [SerializeField] private float speedReductionMultiplier = 10f;
    [SerializeField] private float distortionIntensity = 10f;
    #endregion
    
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        Debug.Log($"Type_Drink_Mystery StartConsumptionEffect");

        // small reduction in movement speed
        Controller_Player.Instance.SetSpeedMoveAndLook(RandShiftVal(speedReductionMultiplier));
        // small increase in hydration
        Controller_Pee.Instance.AddPeeAmount(RandShiftVal(hydrationAmount));
        // small vision distortion
        Manager_Effects.Instance.ApplyVisionDistortion(RandShiftVal(distortionIntensity), RandShiftVal(duration));
        
        // random reduction in all audio source's pitch
        // random visual values
        
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