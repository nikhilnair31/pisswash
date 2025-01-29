using System.Collections.Generic;
using UnityEngine;

public class Controller_Drinker : MonoBehaviour 
{
    private GameObject player;
    private List<Controller_Bottle> theirBottles = new ();
    [SerializeField] private float canSeeInAngle = 110f;

    private void Start() {
        GameObject player = GameObject.FindWithTag("Player");
        CheckForNearbyBottles();
    }

    private void CheckForNearbyBottles() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, LayerMask.GetMask("Interactable"));
        foreach (Collider col in colliders) {
            if (col.TryGetComponent(out Controller_Bottle bottle)) {
                theirBottles.Add(bottle);
                bottle.AddOwner(this);
            }
        }
    }
    
    public bool GetCanSeePlayerStealing() {
        if (!player) return false;

        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0;
        float angle = Vector3.Angle(transform.forward, toPlayer);
        Debug.Log($"Angle: {angle}");
        
        return angle <= canSeeInAngle;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        foreach (Controller_Bottle bottle in theirBottles) {
            Gizmos.DrawLine(transform.position, bottle.transform.position);
        }
    }
}