using UnityEngine;

public class Manager_Audio : MonoBehaviour 
{
    #region Vars
    public static Manager_Audio Instance { get; private set; }
    
    [Header("Ambient Settings")]
    [SerializeField] private AudioSource barAudioSource;
    [SerializeField] private AudioClip barClip;
    [SerializeField] private AudioSource talkingAudioSource;
    [SerializeField] private AudioClip talkingClip;
    
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
    
    public void PlayAudioStain(AudioClip[] clips) {
        PlayRandAudio(stainAudioSource, clips);
    }
    
    public void PlayAudioLevel(AudioClip clip) {
        PlayAudio(levelAudioSource, clip, false);
    }
    public void StopAudioLevel() {
        levelAudioSource.Stop();
    }

    public void PlayAudioTimer(AudioClip clip) {
        PlayAudio(timerAudioSource, clip, true, false);
    }
    public void StopAudioTimer() {
        timerAudioSource.Stop();
    }
    
    private void PlayAudio(AudioSource source, AudioClip clip, bool loop = false, bool randPitch = true) {
        source.clip = clip;
        source.loop = loop;
        if (randPitch)
            source.pitch = GetRandPitchVal();
        source.Play();
    }
    public void PlayRandAudio(AudioSource source, AudioClip[] clips, bool randPitch = true) {
        var clip = clips[Random.Range(0, clips.Length)];
        source.clip = clip;
        if (randPitch)
            source.pitch = GetRandPitchVal();
        source.Play();
    }
    public void PlayRandAudioLoop(AudioSource source, AudioClip[] clips, bool randPitch = true) {
        var clip = clips[Random.Range(0, clips.Length)];
        source.clip = clip;
        if (randPitch)
            source.pitch = GetRandPitchVal();
        source.loop = true;
        source.Play();
    }
    
    private float GetRandPitchVal() {
        return Random.Range(randMinMaxPitch.x, randMinMaxPitch.y);
    }
}