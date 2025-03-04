using UnityEngine;

[CreateAssetMenu(fileName = "Drink", menuName = "Scriptable Objects/Drinks/Drink", order = 1)]
public class Type_Drink : ScriptableObject 
{
    #region Vars
    [Header("Type Settings")]
    [SerializeField] public Manager_Drinks.DrinkType type;

    [Header("Hydration Settings")]
    [SerializeField] public float incrHydrationAmt = 10f;

    [Header("Money Settings")]
    [SerializeField] public int buyCost = 5;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] drinkClips;
    #endregion

    public virtual void StartConsumptionEffect() {
        // Play drinking audio clip
        var peeAudioSource = Controller_Player.Instance.GetAudioSource();
        Manager_Audio.Instance.PlayAudio(peeAudioSource, drinkClips);
    }
}