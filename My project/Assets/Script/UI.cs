using UnityEngine;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour
{
    public Camera mainCamera;
    public Transform player;
    public GameObject uiPanel;
    public float interactionDistance = 3f;
    public LayerMask interactLayer;
    public FirstPersonController firstPersonController;

    private bool isActive = false;

    private void Start()
    {
        if (player == null)
            Debug.LogError("Player not assigned!");

        if (uiPanel != null)
            uiPanel.SetActive(false);
        else
            Debug.LogError("UI Panel not assigned!");

        if (firstPersonController == null)
            Debug.LogWarning("FirstPersonController reference is missing! Please assign it in the inspector.");

    }

    private void Update()
    {
        if (uiPanel == null) return;
        if (firstPersonController == null) return;

        if (Keyboard.current == null)
        {
            Debug.LogWarning("Keyboard.current is null. Make sure Input System is enabled.");
            return;
        }

        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        uiPanel.SetActive(isActive);

        if (distance <= interactionDistance)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                ToggleActive();
            }
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (uiPanel != null && uiPanel.activeSelf)
            {

                if (isActive)
                {
                    RemoveFreeze();
                    isActive = false;
                }

            }
        }
    }

    private void ToggleActive()
    {        
        isActive = !isActive;

        switch (isActive)
        {
            case true:
                AddFreeze();
                break;
            case false:
                RemoveFreeze();
                break;

        }
    }

    private void RemoveFreeze() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (firstPersonController == null)
        {
            Debug.LogWarning("firstPersonController is null!");
        }

        if (firstPersonController != null)
        {
            firstPersonController.UnfreezeMovement(); // разрешение при закрытом UI
        }
    }

    private void AddFreeze()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (firstPersonController == null)
        {
            Debug.LogWarning("firstPersonController is null!");
        }

        if (firstPersonController != null)
        {
            firstPersonController.FreezeMovement(); // заморозка при открытом UI
        }
    }
}
