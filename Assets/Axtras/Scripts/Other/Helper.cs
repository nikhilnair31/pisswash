using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Helper : MonoBehaviour 
{
    public static Helper Instance { get; private set; }
    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    #region Audio Related
    public void PlayRandAudio(AudioSource source, AudioClip[] clips, bool randPitch = true) {
        var clip = clips[Random.Range(0, clips.Length)];
        source.clip = clip;
        if (randPitch)
            source.pitch = Random.Range(0.9f, 1.1f);
        source.Play();
    }
    #endregion
}