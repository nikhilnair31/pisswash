using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class Manager_Audio : MonoBehaviour 
{
    #region Vars
    public static Manager_Audio Instance { get; private set; }
    
    [Header("Ambient Settings")]
    [SerializeField] private AudioMixerSnapshot normalSnapshot;
    [SerializeField] private AudioMixerSnapshot muffledSnapshot;
    [SerializeField] private AudioSource barAudioSource;
    [SerializeField] private AudioClip barClip;
    [SerializeField] private AudioSource talkingAudioSource;
    [SerializeField] private AudioClip talkingClip;
    [SerializeField] private float transitionTime = 1f;
    
    [Header("Stain Settings")]
    [SerializeField] private AudioSource stainAudioSource;
    
    [Header("Controls Settings")]
    [SerializeField] private Vector2 randMinMaxPitch;
    
    [Header("Timer Settings")]
    [SerializeField] private AudioSource levelAudioSource;
    [SerializeField] private AudioSource timerAudioSource;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        PlayAudioAmbient();
    }

    public void PlayAudioAmbient() {
        barAudioSource.clip = barClip;
        barAudioSource.Play();

        talkingAudioSource.clip = talkingClip;
        talkingAudioSource.Play();
    }
    public void ControlAudioAmbient(bool inMenu) {
        if (inMenu) {
            muffledSnapshot.TransitionTo(transitionTime);
        }
        else {
            normalSnapshot.TransitionTo(transitionTime);
        }
    }
    
    public void PlayAudioStain(AudioClip[] clips) {
        PlayAudio(stainAudioSource, clips);
    }

    public void PlayAudioTimer(AudioClip clip, float maxTime) {
        var fadeTween = timerAudioSource
            .DOFade(1f, maxTime);
        fadeTween
            .OnStart(() => {
                timerAudioSource.clip = clip;
                timerAudioSource.loop = true;
                timerAudioSource.volume = 0.5f;
            })
            .OnComplete(() => {
                StopAudioTimer();
            });
    }
    public void StopAudioTimer() {
        timerAudioSource.Stop();
    }
    
    public void PlayAudio(AudioClip clip, bool loop = false, bool randPitch = true) {
        levelAudioSource.clip = clip;
        levelAudioSource.loop = loop;
        if (randPitch)
            levelAudioSource.pitch = GetRandPitchVal();
        levelAudioSource.Play();
    }
    public void PlayAudio(AudioSource source, AudioClip clip, bool loop = false, bool randPitch = true) {
        source.clip = clip;
        source.loop = loop;
        if (randPitch)
            source.pitch = GetRandPitchVal();
        source.Play();
    }
    public void PlayAudio(AudioSource source, AudioClip[] clips, bool loop = false, bool randPitch = true) {
        var clip = clips[Random.Range(0, clips.Length)];
        source.clip = clip;
        if (randPitch)
            source.pitch = GetRandPitchVal();
        source.loop = loop;
        source.Play();
    }
    
    public float GetRandPitchVal() {
        return Random.Range(randMinMaxPitch.x, randMinMaxPitch.y);
    }
}