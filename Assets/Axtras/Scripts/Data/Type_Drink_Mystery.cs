using UnityEngine;

[CreateAssetMenu(fileName = "Mystery Drink", menuName = "Scriptable Objects/Drinks/Mystery", order = 4)]
public class Type_Drink_Mystery : Type_Drink 
{
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        Debug.Log($"Type_Drink_Mystery StartConsumptionEffect");

        // random reduction in movement speed
        // random increase in hydration
        // random vision distortion
        // random reduction in all audio source's pitch
        // random visual values
        
        // random chance of money increase/decrease
        // random chance of slipping
        // random chance of stun/damage
        // random chance of spawning more stains
        // random chance of more fountains
        // random chance of timer increase/decrease
    }
}