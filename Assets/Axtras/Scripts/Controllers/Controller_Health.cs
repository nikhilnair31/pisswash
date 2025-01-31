using UnityEngine;

public class Controller_Health : MonoBehaviour 
{
    #region Vars
    public static Controller_Health Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currHealth = 0;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }    

    private void Start() {
        currHealth = maxHealth;
    }

    public float GetHealthPerc() {
        return (currHealth / maxHealth) * 100f;
    }
}