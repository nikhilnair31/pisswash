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
    private bool isMoving = false;
    private float speedMul = 1f;
    private float camLocalY;

    [Header("Mouse Settings")]
    [SerializeField] private bool canLook = true;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerCamera;
    private float xRotation = 0f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepInterval = 0.5f;
    private float footstepTimer = 0f;
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
        PlayFootsteps();
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

    public void SetCanMoveAndLook(bool active) {
        canMove = active;
        canLook = active;
    }
    public void SetSpeedMoveAndLook(float mul) {
        speedMul *= mul;
    }
}
