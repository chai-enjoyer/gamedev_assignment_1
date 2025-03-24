using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 0.1f;

    [Header("Camera Settings")]
    public Camera playerCamera;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundDistance = 0.4f;

    [Header("UI Reference")]
    public UIController uiController;

    private float xRotation = 0f;
    private bool isGrounded;
    private CharacterController controller;
    private Vector3 velocity;
    private InputSystem_Actions inputActions;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction exitAction;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null) controller = gameObject.AddComponent<CharacterController>();

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogError("No Camera assigned to FirstPersonController!");
                enabled = false;
                return;
            }
        }

        inputActions = new InputSystem_Actions();

        if (uiController == null)
        {
            uiController = Object.FindFirstObjectByType<UIController>();
            if (uiController == null)
            {
                Debug.LogError("UIController not found in scene!");
            }
        }
    }

    void OnEnable()
    {
        moveAction = inputActions.Player.Move;
        lookAction = inputActions.Player.Look;
        jumpAction = inputActions.Player.Jump;
        sprintAction = inputActions.Player.Sprint;
        exitAction = inputActions.Player.Exit;

        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        exitAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
        exitAction.Disable();
    }

    void Update()
    {
        if (!enabled) return;

        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        float speed = sprintAction.IsPressed() ? sprintSpeed : moveSpeed;
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        if (move.magnitude > 1f) move.Normalize();
        controller.Move(move * speed * Time.deltaTime);

        if (jumpAction.WasPressedThisFrame() && isGrounded)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        if (exitAction.WasPressedThisFrame() && uiController != null)
            uiController.ToggleGameplay();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, groundDistance);
    }
}