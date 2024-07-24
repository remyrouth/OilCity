using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingUI : UIState
{
    public override GameState type => GameState.BuildingUI;

    public override void OnUpdate()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        if (EventSystem.current.IsPointerOverGameObject())
            BuildingManager.Instance.CancelBuilding();
        else
            BuildingManager.Instance.OnMouseClick();
    }

}
