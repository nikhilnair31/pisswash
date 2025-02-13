using UnityEngine;

public class Controller_Slippery : MonoBehaviour 
{
    #region Vars
    #endregion

    private void OnTriggerEnter(Collider other) {
        if (GetIsPlayer(other)) {
            StartSippery();
        }
    } 
    private void OnTriggerExit(Collider other) {
        if (GetIsPlayer(other)) {
            StopSippery();
        }        
    }

    private void StartSippery() {
        Manager_Effects.Instance.StartSlipEffectsSeq();
    }
    private void StopSippery() {
        Manager_Effects.Instance.StopSlipEffectsSeq();
    }
    
    private bool GetIsPlayer(Collider other) {
        if (other.CompareTag("Player")) {
            return true;
        }
        return false;
    } 
}