using UnityEngine;

[CreateAssetMenu(fileName = "Whiskey Shot", menuName = "Scriptable Objects/Drinks/Whiskey", order = 3)]
public class Type_Drink_Whiskey : Type_Drink 
{
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        Debug.Log($"Type_Drink_Whiskey StartConsumptionEffect");

        // medium reduction in movement speed
        // medium increase in hydration
        // medium vision distortion
        // medium reduction in all audio source's pitch
        // increased visual saturation and vignette
    }
}