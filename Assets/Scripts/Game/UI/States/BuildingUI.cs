using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingUI : UIState
{
    public override GameState type => GameState.BuildingUI;
    [SerializeField] private BuildingPanelView _buildingPanel;

    private Vector3 _mousePos = Vector2.zero;
    public override void OnUpdate()
    {   

        // while in building mode this allows you to
        // use the mouse to move the camera WITHOUT placing a building
        // or while in building mode use right click to cancel building placement

        // in xbox controller mode we dont have a click drag function, 
        // we're just going to use the stick to move
        // and press B to cancel which is the same as right click up
        if (Input.GetMouseButtonDown(1))
            _mousePos = Input.mousePosition;
        if (Input.GetMouseButtonUp(1))
            if (_mousePos != Vector3.zero && (_mousePos - Input.mousePosition).sqrMagnitude < 1)
                BuildingManager.Instance.CancelBuilding();
    }
    private void Start()
    {
        ControlManager.Instance.leftClickActivationButtontrigger += LeftMouseClick;
        ControlManager.Instance.rightClickActivationButtonTrigger += RightMouseClick;
    }
    private void OnDestroy()
    {
        ControlManager.Instance.leftClickActivationButtontrigger -= LeftMouseClick;
        ControlManager.Instance.rightClickActivationButtonTrigger -= RightMouseClick;
        ControlManager.Instance.rightClickButtonTriggerEnd -= RightMouseClickEnd;
    }

    private void RightMouseClick() {
        if (UIStateMachine.Instance.CurrentStateType != type) {
            return;
        }

        _mousePos = Input.mousePosition;

    }

    private void RightMouseClickEnd() {
        if (UIStateMachine.Instance.CurrentStateType != type) {
            return;
        }
    }

    private void LeftMouseClick()
    {
        if (UIStateMachine.Instance.CurrentStateType != type) {
            return;
        }
        
        // if (Input.GetMouseButtonDown(0))
        // {
        //     if (EventSystem.current.IsPointerOverGameObject())
        //         BuildingManager.Instance.CancelBuilding();
        //     else
        //         BuildingManager.Instance.OnMouseClick();
        // }


        if (ControlManager.Instance.IsCursorOverUIElement()) {
            BuildingManager.Instance.CancelBuilding();
        } else {
            BuildingManager.Instance.OnMouseClick();
        }

    }


    public override void OnEnter()
    {
        base.OnEnter();
        _buildingPanel.Close();
    }

}
