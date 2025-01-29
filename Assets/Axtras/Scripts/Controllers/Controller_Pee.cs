using UnityEngine;
using UnityEngine.UI;

public class Controller_Pee : MonoBehaviour 
{
    #region Vars
    public static Controller_Pee Instance { get; private set; }

    [Header("Pee Settings")]
    [SerializeField] private bool isPeeing = false;
    [SerializeField] private float maxPeeAmount = 100f;
    [SerializeField] private float currPeeAmount;
    [SerializeField] private float peeEmptyRate = 1f;
    [SerializeField] private float peeFillRate = 2f;

    [Header("Fill Settings")]
    [SerializeField] private bool isFilling = false;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem peePS;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] peeClips;

    [Header("UI Settings")]
    [SerializeField] private Image peeImage;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        currPeeAmount = maxPeeAmount;
    }
    private void Update() {
        UpdatePeeAmount();
        UpdateUI();
    }
    
    private void UpdatePeeAmount() {
        if (isPeeing && !isFilling) {
            currPeeAmount -= Time.deltaTime * peeEmptyRate;
        }
        else if (!isPeeing && !isFilling) {
            currPeeAmount += Time.deltaTime * peeFillRate;
        }
        currPeeAmount = Mathf.Clamp(currPeeAmount, 0f, maxPeeAmount);
    }
    public void AddPeeAmount(float amount) {
        currPeeAmount += amount;
    }
    public float GetPeeAmount() {
        return currPeeAmount;
    }

    private void UpdateUI() {
        peeImage.fillAmount = currPeeAmount / maxPeeAmount;
    }

    public void StartPee() {
        peePS.Play();
        Helper.Instance.PlayRandAudio(audioSource, peeClips);
    }
    public void StopPee() {
        peePS.Stop();
    }
    
    public void SetIsPeeing(bool active) {
        isPeeing = active;
        
        if (isPeeing) {
            StartPee();
        }
        else {
            StopPee();
        }
    }
}