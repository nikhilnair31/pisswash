using UnityEngine;

[CreateAssetMenu(fileName = "Beer Bottle", menuName = "Scriptable Objects/Drinks/Beer", order = 2)]
public class Type_Drink_Beer : Type_Drink 
{
    #region Vars
    #endregion
    
    public override void StartConsumptionEffect() {
        base.StartConsumptionEffect();
        
        Debug.Log($"Type_Drink_Beer StartConsumptionEffect");
    }
}