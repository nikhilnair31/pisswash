using UnityEngine;

public class Manager_Audio : MonoBehaviour 
{
    #region Vars
    public static Manager_Audio Instance { get; private set; }
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource barAudioSource;
    [SerializeField] private AudioClip barClip;
    [SerializeField] private AudioSource talkingAudioSource;
    [SerializeField] private AudioClip talkingClip;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        PlayAudio();
    }

    private void PlayAudio() {
        barAudioSource.clip = barClip;
        barAudioSource.Play();

        talkingAudioSource.clip = talkingClip;
        talkingAudioSource.Play();
    }
    
    public void PlayRandAudio(AudioSource source, AudioClip[] clips, bool randPitch = true) {
        var clip = clips[Random.Range(0, clips.Length)];
        source.clip = clip;
        if (randPitch)
            source.pitch = Random.Range(0.9f, 1.1f);
        source.Play();
    }
    
    public void PlayRandAudioLoop(AudioSource source, AudioClip[] clips, bool randPitch = true) {
        var clip = clips[Random.Range(0, clips.Length)];
        source.clip = clip;
        if (randPitch)
            source.pitch = Random.Range(0.9f, 1.1f);
        source.loop = true;
        source.Play();
    }
}