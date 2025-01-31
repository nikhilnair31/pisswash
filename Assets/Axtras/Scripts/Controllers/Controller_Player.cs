using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;

public class Controller_Player : MonoBehaviour
{
    #region Vars
    public static Controller_Player Instance { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float crouchHeight = 1f;
    private Rigidbody rb;
    private Vector3 previousPosition;
    private Vector3 moveDirection;
    private float speedMul = 1f;
    private float camLocalY;

    [Header("Mouse Settings")]
    [SerializeField] private bool canLook = true;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerCamera;
    private float xRotation = 0f;

    [Header("Interactable Settings")]
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask interactableLayer;
    private string showTextStr;
    private RaycastHit hit;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepInterval = 0.5f;
    private float footstepTimer = 0f;
    private bool isMoving = false;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        if (transform.TryGetComponent(out Rigidbody rgb)) {
            rb = rgb;
        }

        rb.freezeRotation = true;
        camLocalY = playerCamera.transform.position.y;
    }

    private void Update() {
        HandleMouseLook();
        HandleMovement();
        HandleInteractable();
        PlayFootsteps();
        CheckForInteractable();
    }

    private void HandleMouseLook() {
        if (!canLook) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * speedMul * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * speedMul * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    private void HandleMovement() {
        if (!canMove) return;
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveDirection = transform.right * x + transform.forward * z;

        isMoving = moveDirection.magnitude > 0;

        if (Input.GetKey(KeyCode.LeftControl)) {
            rb.linearVelocity = new (
                moveDirection.x * crouchSpeed * speedMul, 
                rb.linearVelocity.y, 
                moveDirection.z * crouchSpeed * speedMul
            );
            playerCamera.DOMoveY(camLocalY - crouchHeight, 0.2f);
        } 
        else {
            rb.linearVelocity = new (
                moveDirection.x * moveSpeed * speedMul, 
                rb.linearVelocity.y, 
                moveDirection.z * moveSpeed * speedMul
            );
            playerCamera.DOMoveY(camLocalY, 0.2f);
        }

        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)) {
            rb.linearVelocity = new (
                moveDirection.x * sprintSpeed * speedMul, 
                rb.linearVelocity.y, 
                moveDirection.z * sprintSpeed * speedMul
            );
        }

        rb.AddForce(Vector3.up * gravity);
    }
    private void HandleInteractable() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, interactableLayer)) {
                Debug.Log($"Trying to interact: {hit.transform.name}");

                if (hit.transform.TryGetComponent(out Controller_Bottle bottle)) {
                    bottle.BuyBottle();
                }
                else if (hit.transform.TryGetComponent(out Controller_Fountain fountain)) {
                    fountain.ControlDrinking();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, interactableLayer)) {
                Debug.Log($"Trying to steal : {hit.transform.name}");

                if (hit.transform.TryGetComponent(out Controller_Bottle bottle)) {
                    bottle.StealBottle();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, interactableLayer)) {
                Debug.Log($"Trying to end game : {hit.transform.name}");

                if (hit.transform.TryGetComponent(out Controller_Boss boss)) {
                    boss.FinshRound();
                }
            }
        }
    }

    private void PlayFootsteps() {
        bool hasMoved = Vector3.Distance(transform.position, previousPosition) > 0.01f;
        if (isMoving && hasMoved && rb.linearVelocity.magnitude > 1f) {
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= footstepInterval) {
                Helper.Instance.PlayRandAudio(audioSource, footstepClips);
                footstepTimer = 0f;
            }
        }
        else {
            footstepTimer = 0f;
        }
        previousPosition = transform.position;
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

    public void ControlCanMoveAndLook(bool active) {
        canMove = active;
        canLook = active;
    }
    public void ControlSpeedMoveAndLook(float mul) {
        speedMul *= mul;
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
