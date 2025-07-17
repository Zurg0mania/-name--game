using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

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

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();

        // Подписка на движение
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
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
        // Считываем дельту движения мыши прямо в Update
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        // Свободный поворот игрока вокруг оси Y (влево-вправо)
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        // Очень небольшой наклон камеры вверх-вниз
        pitch -= lookInput.y * lookSensitivity * 0.1f;
        pitch = Mathf.Clamp(pitch, -25f, 25f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        if (controller.isGrounded)
            verticalVelocity = -0.5f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        // Горизонтальное направление камеры (без вертикальной компоненты)
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
}
