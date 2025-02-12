using System.Collections.Generic;
using UnityEngine;

public class Controller_Drinker : Controller_Person 
{
    #region Variables
    [SerializeField] private Transform seeFromTransform;
    [SerializeField] private float canSeeInRange = 5f;
    [SerializeField] private float canSeeInAngle = 110f;
    private List<Controller_Drink> theirDrinks = new ();
    #endregion

    protected override void Start() {
        base.Start();
        
        CheckForNearbyDrinks();
    }

    private void CheckForNearbyDrinks() {
        Collider[] colliders = Physics.OverlapSphere(seeFromTransform.position, canSeeInRange, LayerMask.GetMask("Interactable"));
        foreach (Collider col in colliders) {
            if (col.TryGetComponent(out Controller_Drink Drink)) {
                theirDrinks.Add(Drink);
                Drink.AddOwner(this);
            }
        }
    }
    
    public bool GetCanSeePlayerStealing() {
        if (!player) return false;

        Vector3 toPlayer = player.transform.position - seeFromTransform.position;
        toPlayer.y = 0;
        float angle = Vector3.Angle(seeFromTransform.forward, toPlayer);
        Debug.Log($"Angle: {angle}");

        return angle <= canSeeInAngle;
    }
    public Transform GetPlayerSeeSource() {
        return seeFromTransform;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        foreach (Controller_Drink Drink in theirDrinks) {
            Gizmos.DrawLine(seeFromTransform.position, Drink.transform.position);
        }
    }
}