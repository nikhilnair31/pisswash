using UnityEngine;

[CreateAssetMenu(fileName = "Stain", menuName = "Scriptable Objects/Stains/Stain", order = 1)]
public class Type_Stain : ScriptableObject 
{
    [Header("Type Settings")]
    [SerializeField] public Manager_Stains.StainType type;

    [Header("Fade Settings")]
    [SerializeField] public float fadeAmountPerCollision = 0.01f;

    public virtual void StartInteractionEffect() {
        Debug.Log($"Stain StartInteractionEffect");
    }
}