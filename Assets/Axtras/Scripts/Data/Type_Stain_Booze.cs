using UnityEngine;

[CreateAssetMenu(fileName = "Booze", menuName = "Scriptable Objects/Stains/Booze", order = 1)]
public class Type_Stain_Booze : Type_Stain 
{
    public virtual void StartInteractionEffect() {
        Debug.Log($"Booze StartInteractionEffect");
    }
}