using UnityEngine;
using DG.Tweening;

public class Manager_Timer : MonoBehaviour 
{
    #region Vars
    public static Manager_Timer Instance { get; private set; }
    
    [Header("Timer Settings")]
    [SerializeField] private float maxTime = 60f;
    private float timer = 0f;
    private bool isRunning = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource timerAudioSource;
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

        timerAudioSource.clip = countingDownClip;
        timerAudioSource.volume = 0.1f;
        timerAudioSource.loop = true;
        timerAudioSource.Play();

        timerAudioSource
            .DOFade(1f, maxTime)
            .OnComplete(() => {
                timerAudioSource.Stop();
            });
    }
    public void StopTimer(bool timeUp) {
        isRunning = false;

        if (timeUp) {
            timerAudioSource.pitch = 1f;
            timerAudioSource.clip = timeUpClip;
            timerAudioSource.loop = true;
            timerAudioSource.Play();

            DOVirtual.DelayedCall(3f, () => {
                timerAudioSource.Stop();
            });
        }
        else {
            timerAudioSource.Stop();
        }
    }

    public void AddTimerAmt(float val) {
        timer += val;
    }

    public float GetTimeRemainingPerc() {
        return (timer / maxTime) * 100f;
    }
}