using UnityEngine;

[CreateAssetMenu(fileName = "Whiskey Shot", menuName = "Scriptable Objects/Drinks/Whiskey", order = 3)]
public class Type_Drink_Whiskey : Type_Drink 
{
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        Debug.Log($"Type_Drink_Whiskey StartConsumptionEffect");
    }
}