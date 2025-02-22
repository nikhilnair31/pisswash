using UnityEngine;

public class Controller_Fountain : Controller_Interactables 
{
    #region Vars
    [Header("Hydration Settings")]
    [SerializeField] private bool isOn = false;
    [SerializeField] private float increaseHydrationRate = 2f;

    [Header("Effect Settings")]
    [SerializeField] private ParticleSystem fountainEffect;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] drinkClips;
    #endregion

    private void FixedUpdate() {
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
            fountainEffect.Play();
            Manager_Audio.Instance.PlayAudio(audioSource, drinkClips, true);
        }
        else {
            fountainEffect.Stop();
            audioSource.Stop();
        }
    }
}