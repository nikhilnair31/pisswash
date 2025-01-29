using UnityEngine;

public class Controller_Fountain : Controller_Interactables 
{
    #region Vars
    [Header("Hydration Settings")]
    [SerializeField] private bool isOn = false;
    [SerializeField] private float increaseHydrationRate = 2f;
    #endregion

    private void Update() {
        if (isOn) {
            if (Controller_Pee.Instance.GetPeeAmount() > 0) {
                Controller_Pee.Instance.AddPeeAmount(increaseHydrationRate * Time.deltaTime);
            }
            else {
                isOn = false;
            }
        }
    }

    public void ControlDrinking() {
        isOn = !isOn;
    }
}