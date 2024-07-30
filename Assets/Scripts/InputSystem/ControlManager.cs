using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{
    public Image mouseCursorUI; 
    public RectTransform canvasRectTransform;

    private PlayerInput playerControls;

    private Camera mainCamera;

    

    private void Update() {
        // Debug.Log(mouseCursorUI.gameObject.transform.position);
    }

    private void Awake() {
        mainCamera = Camera.main;
        playerControls = new PlayerInput();

        playerControls.PlayerControls.MoveCursor.performed += OnMouseMove;
    }

    void OnMouseMove(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
        Vector2 viewportPosition = mainCamera.ScreenToViewportPoint(mousePosition);
        Vector2 worldObjectScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRectTransform.sizeDelta.x) - (canvasRectTransform.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRectTransform.sizeDelta.y) - (canvasRectTransform.sizeDelta.y * 0.5f)));

        mouseCursorUI.rectTransform.anchoredPosition = worldObjectScreenPosition;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
