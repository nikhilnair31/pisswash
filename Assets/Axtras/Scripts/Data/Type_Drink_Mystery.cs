using UnityEngine;

[CreateAssetMenu(fileName = "Mystery Drink", menuName = "Scriptable Objects/Drinks/Mystery", order = 4)]
public class Type_Drink_Mystery : Type_Drink 
{
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        Debug.Log($"Type_Drink_Mystery StartConsumptionEffect");
    }
}