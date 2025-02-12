using UnityEngine;

[CreateAssetMenu(fileName = "Drink", menuName = "Scriptable Objects/Drinks/Drink", order = 1)]
public class Type_Drink : ScriptableObject 
{
    [Header("Type Settings")]
    [SerializeField] public Manager_Drinks.DrinkType type;

    [Header("Hydration Settings")]
    [SerializeField] public float increaseHydrationAmount = 10f;

    [Header("Money Settings")]
    [SerializeField] public int buyCost = 5;

    public virtual void StartConsumptionEffect() {
        Debug.Log($"Drink StartConsumptionEffect");
    }
}