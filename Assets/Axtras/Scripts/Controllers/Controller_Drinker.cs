using System.Collections.Generic;
using UnityEngine;

public class Controller_Drinker : MonoBehaviour 
{
    #region Variables
    [SerializeField] private Transform seeFromTransform;
    [SerializeField] private float canSeeInRange = 5f;
    [SerializeField] private float canSeeInAngle = 110f;
    private List<Controller_Bottle> theirBottles = new ();
    private GameObject player;
    #endregion

    private void Start() {
        player = GameObject.FindWithTag("Player");

        CheckForNearbyBottles();
    }

    private void CheckForNearbyBottles() {
        Collider[] colliders = Physics.OverlapSphere(seeFromTransform.position, canSeeInRange, LayerMask.GetMask("Interactable"));
        foreach (Collider col in colliders) {
            if (col.TryGetComponent(out Controller_Bottle bottle)) {
                theirBottles.Add(bottle);
                bottle.AddOwner(this);
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
        foreach (Controller_Bottle bottle in theirBottles) {
            Gizmos.DrawLine(seeFromTransform.position, bottle.transform.position);
        }
    }
}