using UnityEngine;

public class Manager_Timer : MonoBehaviour 
{
    #region Vars
    public static Manager_Timer Instance { get; private set; }
    
    [SerializeField] private float maxTime = 60f;
    private float timer = 0f;
    private bool isRunning = false;
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
            Manager_UI.Instance.SetTimer(timer);
        }
    }

    public void StartTimer() {
        isRunning = true;
    }
    public void StopTimer() {
        isRunning = false;
    }
    public void ResetTimer() {
        timer = 0f;
    }   
}