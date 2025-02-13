using UnityEngine;
using System.Collections;

public class Controller_Slippery : MonoBehaviour 
{
    #region Vars
    [Header("Settings")]
    [SerializeField] private float forceMagnitude = 2f;
    [SerializeField] private float maxStayTime = 2f;
    private float stayTime;
    #endregion

    private void OnTriggerEnter(Collider other) {
        if (GetIsPlayer(other)) {
            StartSippery(other);
        }
    } 
    private void OnTriggerStay(Collider other) {
        if (GetIsPlayer(other)) {
            stayTime += Time.deltaTime;
            if (stayTime >= maxStayTime) {
                stayTime = 0f;
                StartCoroutine(EnableColliderAfterDelay(2f));
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (GetIsPlayer(other)) {
            StopSippery(other);
        }        
    }

    private void StartSippery(Collider player) {
        Manager_Effects.Instance.StartSlipEffectsSeq(1f);

        //  Adding move force
        if (player.TryGetComponent(out Rigidbody rb)) {
            Vector3 forceDirection = rb.linearVelocity.normalized;
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        }
    }
    private void StopSippery(Collider player) {
        Manager_Effects.Instance.StopSlipEffectsSeq();
    }

    private IEnumerator EnableColliderAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        GetComponent<Collider>().enabled = true;
    }
    
    private bool GetIsPlayer(Collider other) {
        if (other.CompareTag("Player")) {
            return true;
        }
        return false;
    } 
}