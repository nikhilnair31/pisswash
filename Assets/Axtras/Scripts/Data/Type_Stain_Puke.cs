using UnityEngine;

[CreateAssetMenu(fileName = "Puke", menuName = "Scriptable Objects/Stains/Puke", order = 1)]
public class Type_Stain_Puke : Type_Stain 
{
    #region Vars
    [Header("Puke Properties")]
    [SerializeField] private float scaleAmount = 1.2f;
    [SerializeField] private float scaleTime = 5f;

    #endregion
    
    public override void StartInteractionEffect() {
        base.StartInteractionEffect();
    }
}