using UnityEngine;

public class Controller_Interactables : MonoBehaviour 
{
    #region Vars
    internal Controller_Player playerController;

    [Header("Component Settings")]
    [SerializeField] private bool addAudioSource;
    [SerializeField] private bool addRigidbody;
    [SerializeField] private bool addMeshCollider;
    [SerializeField] private bool addBoxCollider;
    internal AudioSource audioSource;
    internal Rigidbody rgb;
    internal MeshCollider meshColl;
    internal BoxCollider boxColl;

    [Header("Interaction Settings")]
    [SerializeField] internal string showThisText;
    [SerializeField] internal float showForTime = -1f;
    #endregion

    private void Awake() {
        playerController = FindFirstObjectByType<Controller_Player>();

        if (!transform.TryGetComponent(out AudioSource source) && addAudioSource) {
            audioSource = transform.gameObject.AddComponent<AudioSource>();
        } 
        else {
            audioSource = source;
        }
        if (!transform.TryGetComponent(out Rigidbody rb) && addRigidbody) {
            rgb = transform.gameObject.AddComponent<Rigidbody>();
        } 
        else {
           rgb = rb;
        }
        if (!transform.TryGetComponent(out MeshCollider mc) && addMeshCollider) {
            meshColl = transform.gameObject.AddComponent<MeshCollider>();
            if (rgb != null) {
                meshColl.convex = true;
            }
        } 
        else {
            meshColl = mc;
        }
        if (!transform.TryGetComponent(out BoxCollider bc) && addBoxCollider) {
            boxColl = transform.gameObject.AddComponent<BoxCollider>();
        } 
        else {
            boxColl = bc;
        }

        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
    
    public void SetInteractionText(string newtext) {
        showThisText = newtext;
    }

    public (string text, float duration) ReturnInfo() {
        return new(showThisText, showForTime);
    }
}