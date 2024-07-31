using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControlManager : Singleton<ControlManager>
{
    public Image mouseCursorUI; 
    public RectTransform canvasRectTransform;

    private PlayerInput playerControls;
    private Camera mainCamera;
    private InputType lastUsedInput = InputType.None;
    public delegate void Buttontrigger();
    public event Buttontrigger leftClickActivationButtontrigger;
    private Vector2 lastMousePosition;
    private Vector2 lastStickInput;
    private enum InputType
    {
        None,
        Mouse,
        Stick
    }

    private float horizontalInput = 0f;
    private float verticalInput = 0f;

    private void Awake() 
    {
        mainCamera = Camera.main;
        playerControls = new PlayerInput();

        playerControls.PlayerControls.MoveCursor.performed += OnMouseMove;
        playerControls.PlayerControls.MoveLeftStick.performed += OnLeftStickMove;
        playerControls.PlayerControls.MoveLeftStick.canceled += LeftStickUsageUpdate;
        playerControls.PlayerControls.ClickSelect.performed += OnSelectionButtonTrigger;



        playerControls.PlayerControls.MoveLeftStickHorizontal.performed += ctx => {
            horizontalInput = ctx.ReadValue<float>();
            // You can use horizontalInput for left/right turning here
        };

        playerControls.PlayerControls.MoveLeftStickVertical.performed += ctx => {
            verticalInput = ctx.ReadValue<float>();
            // You can use verticalInput for up/down turning here
        };

        Cursor.visible = false;
    }

    private void Update()
    {
        UpdateCursorPosition(lastMousePosition);

    }

    private void LeftStickUsageUpdate(InputAction.CallbackContext context)
    {
        lastUsedInput = InputType.None;
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        lastUsedInput = InputType.Mouse;
        lastMousePosition = context.ReadValue<Vector2>();
    }

    private void OnLeftStickMove(InputAction.CallbackContext context)
    {
        lastUsedInput = InputType.Stick;
        lastStickInput = context.ReadValue<Vector2>();
    }

    private void OnSelectionButtonTrigger(InputAction.CallbackContext context)
    {
        leftClickActivationButtontrigger?.Invoke();
    }

    private void UpdateCursorPosition(Vector2 input)
    {
        Vector2 newPosition = mouseCursorUI.rectTransform.anchoredPosition;

        switch (lastUsedInput)
        {
            case InputType.Mouse:
                Vector2 viewportPosition = mainCamera.ScreenToViewportPoint(input);
                newPosition = new Vector2(
                    ((viewportPosition.x * canvasRectTransform.sizeDelta.x) - (canvasRectTransform.sizeDelta.x * 0.5f)),
                    ((viewportPosition.y * canvasRectTransform.sizeDelta.y) - (canvasRectTransform.sizeDelta.y * 0.5f)));
                break;
            case InputType.Stick:
                Vector2 individualizedVector2 = new Vector2(horizontalInput, verticalInput) * 10f;
                newPosition = mouseCursorUI.rectTransform.anchoredPosition + (individualizedVector2);
                break;
            case InputType.None:
                newPosition = mouseCursorUI.rectTransform.anchoredPosition;
                // UpdateCursorPosition(lastStickInput, false);
                break;
        }

        // Clamp the position to the canvas boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, -canvasRectTransform.sizeDelta.x * 0.5f, canvasRectTransform.sizeDelta.x * 0.5f);
        newPosition.y = Mathf.Clamp(newPosition.y, -canvasRectTransform.sizeDelta.y * 0.5f, canvasRectTransform.sizeDelta.y * 0.5f);

        mouseCursorUI.rectTransform.anchoredPosition = newPosition;
    }

    public Vector3 RetrieveMousePosition() 
    {
        return mouseCursorUI.gameObject.transform.position;
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
