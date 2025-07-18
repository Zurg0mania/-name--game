using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 5f;

    [Header("Look")]
    public Transform cameraTransform;
    public float lookSensitivity = 1.5f;

    private CharacterController controller;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private float pitch = 0f;

    private float verticalVelocity = 0f;
    public float gravity = -9.81f;

    private bool jumpRequested = false;
    private bool canMove = true;

    public void EnableInput()
    {
        inputActions.Enable();
    }

    public void DisableInput()
    {
        inputActions.Disable();
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();

        // Подписка на движение
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Подписка на прыжок
        inputActions.Player.Jump.performed += ctx => jumpRequested = true;
    }

    void OnEnable()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();

        inputActions.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        if (inputActions != null)
            inputActions.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!canMove)
            return;
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        pitch -= lookInput.y * lookSensitivity * 0.1f;
        pitch = Mathf.Clamp(pitch, -25f, 25f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        if (controller.isGrounded)
        {
            if (jumpRequested)
            {
                verticalVelocity = jumpForce;
                jumpRequested = false;
            }
            else if (verticalVelocity < 0f)
            {
                verticalVelocity = -1f; // чтобы не "прилипал"
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = forward * moveInput.y + right * moveInput.x;
        move = move.normalized * moveSpeed;

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
    public void FreezeMovement()
    {
        Debug.Log("Can move start: " + canMove);
        canMove = false;
        Debug.Log("Can move end: " + canMove);
    }

    public void UnfreezeMovement()
    {
        canMove = true;
    }
}
