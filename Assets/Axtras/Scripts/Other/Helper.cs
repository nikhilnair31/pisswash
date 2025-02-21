using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Helper : MonoBehaviour 
{
    public static Helper Instance { get; private set; }
    
    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool GetIsPlayer(Collider other) {
        if (other.CompareTag("Player")) {
            return true;
        }
        return false;
    }
    public bool GetIsPlayer(Collision other) {
        if (other.transform.CompareTag("Player")) {
            return true;
        }
        return false;
    } 

    #region Random Related
    public bool TriggerBool(float chance) {
        return Random.Range(0f, 1f) >= chance;
    }

    public float RandShiftVal(float val, float randPerc = 0.2f) {
        var percAmt = val * randPerc;
        return val + Random.Range(-percAmt, percAmt);
    }
    public int RandShiftVal(int val, float randPerc = 0.2f) {
        var percAmt = val * randPerc;
        var newVal = val + Random.Range(-percAmt, percAmt);
        return (int)newVal;
    }
    #endregion
}