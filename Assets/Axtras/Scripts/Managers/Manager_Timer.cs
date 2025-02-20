using UnityEngine;

public class Manager_Timer : MonoBehaviour 
{
    #region Vars
    public static Manager_Timer Instance { get; private set; }
    
    [Header("Timer Settings")]
    [SerializeField] private float maxTime = 60f;
    private float timer = 0f;
    private bool isRunning = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip countingDownClip;
    [SerializeField] private AudioClip levelStartClip;
    [SerializeField] private AudioClip levelOverClip;
    [SerializeField] private AudioClip timeUpClip;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        timer = maxTime;
    }

    private void Update() {
        if (isRunning) {
            timer -= Time.deltaTime;
            Manager_UI.Instance.SetTimerUI(timer);

            if (timer <= 0f) {
                StopTimer(true);
                Manager_UI.Instance.SetTimerUI(0f);
                Manager_UI.Instance.LevelOver();
            }
        }
    }

    public void StartTimer() {
        isRunning = true;
        Manager_Audio.Instance.PlayAudioTimer(countingDownClip);
        Manager_Audio.Instance.PlayAudioLevel(levelStartClip);
    }
    public void StopTimer(bool timeUp) {
        isRunning = false;

        if (timeUp) {
            Manager_Audio.Instance.PlayAudioLevel(timeUpClip);
        }
        else {
            Manager_Audio.Instance.StopAudioLevel();
        }
    }
    public void ResetTimer() {
        timer = 0f;
    }
    
    public void AddTimerAmt(float val) {
        timer += val;
    }

    public float GetTimeRemainingPerc() {
        return (timer / maxTime) * 100f;
    }
}