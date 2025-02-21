using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;

public class Controller_Player : MonoBehaviour
{
    #region Vars
    public static Controller_Player Instance { get; private set; }

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isMoving = false;
    private float speedMul = 1f;
    private float xRotation = 0f;
    private float footstepTimer = 0f;

    [Header("Movement Settings")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Settings")]
    [SerializeField] private bool canLook = true;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerCamera;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepInterval = 0.5f;

    [Header("Tutorial Settings")]
    [SerializeField] private string tutorialPlayerPrefsStr = "Tutorials-Player-MoveLook";
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

        Manager_Tutorials.Instance.PlayTutorial(tutorialPlayerPrefsStr);
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

        rb.linearVelocity = new (
            moveDirection.x * moveSpeed * speedMul, 
            rb.linearVelocity.y, 
            moveDirection.z * moveSpeed * speedMul
        );

        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)) {
            rb.linearVelocity = new (
                moveDirection.x * sprintSpeed * speedMul, 
                rb.linearVelocity.y, 
                moveDirection.z * sprintSpeed * speedMul
            );
        }
    }

    private void PlayFootsteps() {
        if (isMoving) {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval) {
                Manager_Audio.Instance.PlayAudio(audioSource, footstepClips);
                footstepTimer = 0f;
            }
        }
        else {
            footstepTimer = 0f;
        }
    }

    public AudioSource GetAudioSource() {
        return audioSource;
    }

    public void SetCanMoveAndLook(bool active) {
        canMove = active;
        canLook = active;
    }
    public void SetSpeedMoveAndLook(float mul) {
        speedMul = mul;
    }
}
