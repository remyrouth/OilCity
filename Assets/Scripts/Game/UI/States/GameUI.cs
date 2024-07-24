using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : UIState
{
    public override GameState type => GameState.GameUI;
    public override void OnUpdate()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        if (!EventSystem.current.IsPointerOverGameObject())
            TileSelector.Instance.OnMouseClick();
    }
}
