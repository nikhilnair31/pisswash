using UnityEngine;

[CreateAssetMenu(fileName = "Type_Stain", menuName = "Scriptable Objects/Stains", order = 0)]
public class Type_Stain : ScriptableObject 
{
    [Header("Type Settings")]
    [SerializeField] public Controller_Stain.StainType type;

    [Header("Fade Settings")]
    [SerializeField] public float fadeAmountPerCollision = 0.01f;
}