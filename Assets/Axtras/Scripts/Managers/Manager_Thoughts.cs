using System.Collections;
using UnityEngine;

public class Manager_Thoughts : MonoBehaviour 
{
    #region Vars
    public static Manager_Thoughts Instance { get; private set; }

    public enum TextPriority { None, Item, Collider, Player }
    
    [Header("Thoughts Settings")]
    [SerializeField] private AudioSource thoughtsAudioSource;
    [SerializeField] private AudioClip[] typingClips;
    [SerializeField] private float showTypingSpeed = 0.05f;
    [SerializeField] private float hideTypingSpeed = 0.02f;
    
    private TextPriority currentTextPriority = TextPriority.None;
    private Coroutine currentShowTextCoroutine;
    private Coroutine currentClearTextCoroutine;
    private string currentText;
    private float currentShowTime;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowText(string text, float showTime, TextPriority priority) {
        // If new priority is lower than current, don't override
        if (priority < currentTextPriority) return;

        // If same priority but different text, or higher priority
        if (priority >= currentTextPriority) {
            // Stop any existing coroutines
            if (currentShowTextCoroutine != null) {
                StopCoroutine(currentShowTextCoroutine);
            }
            if (currentClearTextCoroutine != null) {
                StopCoroutine(currentClearTextCoroutine);
            }

            currentTextPriority = priority;
            currentText = text;
            currentShowTime = showTime;
            
            currentShowTextCoroutine = StartCoroutine(ShowTextSequence(text, showTime, priority));
        }
    }

    private IEnumerator ShowTextSequence(string text, float showTime, TextPriority priority) {
        Manager_UI.Instance.SetShowText(text);

        // Only start auto-clear timer if showTime is greater than 0
        if (showTime > 0) {
            currentClearTextCoroutine = StartCoroutine(AutoClearText(showTime, priority));
        }

        yield return null;
    }

    private IEnumerator AutoClearText(float delay, TextPriority priority) {
        yield return new WaitForSeconds(delay);
        
        // Only clear if the priority matches (hasn't been overridden)
        if (currentTextPriority == priority) {
            ClearThoughtText(priority);
        }
    }

    public void ClearThoughtText(TextPriority priority) {
        // Only clear if the priority matches or is higher
        if (priority >= currentTextPriority) {
            if (currentShowTextCoroutine != null) {
                StopCoroutine(currentShowTextCoroutine);
            }
            if (currentClearTextCoroutine != null) {
                StopCoroutine(currentClearTextCoroutine);
            }

            currentShowTextCoroutine = null;
            currentClearTextCoroutine = null;
            currentTextPriority = TextPriority.None;
            currentText = string.Empty;
            currentShowTime = 0f;
            
            Manager_UI.Instance.ClearShowText();
        }
    }

    // Helper method to get current text state
    public (string text, TextPriority priority, float remainingTime) GetCurrentTextState() {
        return (currentText, currentTextPriority, currentShowTime);
    }
}