using UnityEngine;

public class Controller_Person : MonoBehaviour 
{
    #region Variables
    [SerializeField] protected MeshRenderer rend;
    [SerializeField] protected GameObject player;

    [SerializeField] private Material[] skinMats;
    protected Material mat;

    [Header("Audio Settings")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] private AudioClip[] ewwClips;
    #endregion

    protected virtual void Start() {
        if (rend == null)
            TryGetComponent(out rend);
        if (audioSource == null)
            TryGetComponent(out audioSource);
        if (player == null)
            player = GameObject.FindWithTag("Player");
        
        int randInd = Random.Range(0, skinMats.Length);
        rend.sharedMaterial = skinMats[randInd];
    }

    public void AddPeedOnSound() {
        // Play eww audio clip
        Manager_Audio.Instance.PlayAudio(audioSource, ewwClips);
    }
}