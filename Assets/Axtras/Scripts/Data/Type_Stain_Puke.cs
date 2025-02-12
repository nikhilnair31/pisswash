using UnityEngine;

[CreateAssetMenu(fileName = "Puke", menuName = "Scriptable Objects/Stains/Puke", order = 1)]
public class Type_Stain_Puke : Type_Stain 
{
    public virtual void StartInteractionEffect() {
        Debug.Log($"Puke StartInteractionEffect");
    }
}