using UnityEngine;

public class Controller_Fountain : Controller_Interactables 
{
    #region Vars
    [Header("Hydration Settings")]
    [SerializeField] private bool isOn = false;
    [SerializeField] private float increaseHydrationRate = 2f;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] drinkClips;
    #endregion

    private void Update() {
        if (isOn) {
            if (!Controller_Pee.Instance.GetIsPeeFull()) {
                Controller_Pee.Instance.AddPeeAmount(increaseHydrationRate * Time.deltaTime);
            }
            else {
                isOn = false;
            }
        }
    }

    public void ControlDrinking(bool active) {
        isOn = active;
        if (isOn) {
            Helper.Instance.PlayRandAudioLoop(audioSource, drinkClips);
        }
        else {
            audioSource.Stop();
        }
    }
}