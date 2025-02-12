using UnityEngine;

[CreateAssetMenu(fileName = "Type_Drink", menuName = "Scriptable Objects/Drinks", order = 0)]
public class Type_Drink : ScriptableObject 
{
    [Header("Type Settings")]
    [SerializeField] public Controller_Drink.DrinkType type;

    [Header("Hydration Settings")]
    [SerializeField] public float increaseHydrationAmount = 10f;

    [Header("Money Settings")]
    [SerializeField] public int buyCost = 5;
}