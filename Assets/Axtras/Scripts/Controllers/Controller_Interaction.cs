using UnityEngine;
using Unity.Cinemachine;

public class Controller_Interaction : MonoBehaviour 
{
    #region Vars
    public static Controller_Interaction Instance { get; private set; }

    [Header("General Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask interactableLayer;
    private string showTextStr;
    private RaycastHit hit;

    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    private Controller_Drink currentDrink;
    private Controller_Fountain currentFountain;

    [Header("Stealing Settings")]
    [SerializeField] private KeyCode stealKey = KeyCode.F;

    [Header("Round End Settings")]
    [SerializeField] private KeyCode endRoundKey = KeyCode.G;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<CinemachineCamera>().transform;
    }

    private void Update() {  
        HandleInteractable();
        CheckForInteractable();
    }

    private void HandleInteractable() {
        // Interacting key
        if (Input.GetKeyDown(interactKey)) {
            Debug.Log($"Interacting: {hit.transform}");
            if (hit.transform.TryGetComponent(out Controller_Drink drink)) {
                currentDrink = drink;
                drink.ConsumeDrink(buyOrSteal: "buy");
            }
            else if (hit.transform.TryGetComponent(out Controller_Fountain fountain)) {
                currentFountain = fountain;
                fountain.ControlDrinking(true);
            }
        }
        else if (Input.GetKeyUp(interactKey)) {
            if (currentDrink != null) {
            }
            if (currentFountain != null) {
                currentFountain.ControlDrinking(false);
                currentFountain = null;
            }
        }

        // Stealing key
        if (Input.GetKeyDown(stealKey)) {
            Debug.Log("Stealing Drink");
            if (hit.transform.TryGetComponent(out Controller_Drink drink)) {
                drink.ConsumeDrink(buyOrSteal: "steal");
            }
        }
        
        // Ending game key
        if (Input.GetKeyDown(endRoundKey)) {
            Debug.Log("Ending round");
            if (hit.transform.TryGetComponent(out Controller_Boss boss)) {
                boss.FinshRound();
            }
        }
    }
    
    private void CheckForInteractable() {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, interactableLayer)) {
            if (hit.transform.TryGetComponent(out Controller_Interactables interactable)) {
                // Get info
                var (text, duration) = interactable.GetInfo();
                if (text != showTextStr) {
                    showTextStr = text;
                    Manager_Thoughts.Instance.ShowText(
                        text, 
                        duration,
                        Manager_Thoughts.TextPriority.Item
                    );
                }
                return;
            }
        }

        // If no interactable is detected or conditions aren't met, clear text
        showTextStr = null;
        Manager_Thoughts.Instance.ClearThoughtText(
            Manager_Thoughts.TextPriority.Item
        );
    }

    public string GetInteractKey() {
        return interactKey.ToString();
    }
    public string GetStealKey() {
        return stealKey.ToString();
    }

    private void OnDrawGizmos() {
        // Only draw the Gizmos if the ray has hit something
        if (hit.collider != null) {
            // Draw the ray as a line
            Gizmos.color = Color.red;
            Gizmos.DrawLine(hit.point - hit.normal * 0.5f, hit.point);

            // Draw a sphere at the hit point
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }    
}