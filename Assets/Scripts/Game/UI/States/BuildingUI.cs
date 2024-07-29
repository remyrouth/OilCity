using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingUI : UIState
{
    public override GameState type => GameState.BuildingUI;
    [SerializeField] private BuildingPanelView _buildingPanel;

    private Vector3 _mousePos = Vector2.zero;
    public override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                BuildingManager.Instance.CancelBuilding();
            else
                BuildingManager.Instance.OnMouseClick();
        }
        if (Input.GetMouseButtonDown(1))
            _mousePos = Input.mousePosition;
        if(Input.GetMouseButtonUp(1))
            if(_mousePos != Vector3.zero && (_mousePos-Input.mousePosition).sqrMagnitude<1)
                BuildingManager.Instance.CancelBuilding();
    }
    public override void OnEnter()
    {
        base.OnEnter();
        _buildingPanel.Close();
    }

}
