using UnityEngine;

public class Controller_Interaction : MonoBehaviour 
{
    #region Vars
    public static Controller_Interaction Instance { get; private set; }

    [Header("Interactable Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask interactableLayer;
    private string showTextStr;
    private RaycastHit hit;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update() {  
        HandleInteractable();
        CheckForInteractable();
    }

    private void HandleInteractable() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log($"Trying to interact: {hit.transform.name}");

            if (hit.transform.TryGetComponent(out Controller_Bottle bottle)) {
                bottle.BuyBottle();
            }
            else if (hit.transform.TryGetComponent(out Controller_Fountain fountain)) {
                fountain.ControlDrinking();
            }
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            Debug.Log($"Trying to steal : {hit.transform.name}");

            if (hit.transform.TryGetComponent(out Controller_Bottle bottle)) {
                bottle.StealBottle();
            }
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            Debug.Log($"Trying to end game : {hit.transform.name}");

            if (hit.transform.TryGetComponent(out Controller_Boss boss)) {
                boss.FinshRound();
            }
        }
    }
    
    private void CheckForInteractable() {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, interactableLayer)) {
            if (hit.transform.TryGetComponent(out Controller_Interactables interactable)) {
                var (text, duration) = interactable.ReturnInfo();
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