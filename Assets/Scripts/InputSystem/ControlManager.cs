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

    private void Awake() {
        playerControls = new PlayerInput();
    }

    public void MoveHorizontal(InputAction.CallbackContext context)
    {
        // Get the value from the input context
        float delta = context.ReadValue<float>();

        // Calculate the new position
        Vector2 newPosition = mouseCursorUI.rectTransform.anchoredPosition;
        newPosition.x += delta;

        // Clamp the position to keep the cursor within the bounds of the canvas
        float canvasWidth = canvasRectTransform.rect.width;
        float cursorWidth = mouseCursorUI.rectTransform.rect.width;
        newPosition.x = Mathf.Clamp(newPosition.x, 0, canvasWidth - cursorWidth);

        // Apply the new position
        mouseCursorUI.rectTransform.anchoredPosition = newPosition;
    }

    public void MoveVertical(InputAction.CallbackContext context)
    {
        // Get the value from the input context
        float delta = context.ReadValue<float>();

        // Calculate the new position
        Vector2 newPosition = mouseCursorUI.rectTransform.anchoredPosition;
        newPosition.y += delta;

        // Clamp the position to keep the cursor within the bounds of the canvas
        float canvasHeight = canvasRectTransform.rect.height;
        float cursorHeight = mouseCursorUI.rectTransform.rect.height;
        newPosition.y = Mathf.Clamp(newPosition.y, 0, canvasHeight - cursorHeight);

        // Apply the new position
        mouseCursorUI.rectTransform.anchoredPosition = newPosition;
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
