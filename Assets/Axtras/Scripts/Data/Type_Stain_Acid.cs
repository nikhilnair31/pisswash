using UnityEngine;

[CreateAssetMenu(fileName = "Acid", menuName = "Scriptable Objects/Stains/Acid", order = 1)]
public class Type_Stain_Acid : Type_Stain 
{
    public virtual void StartInteractionEffect() {
        Debug.Log($"Acid StartInteractionEffect");
    }
}