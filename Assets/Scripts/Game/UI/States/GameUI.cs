using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : UIState
{
    public override GameState type => GameState.GameUI;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private BuildingPanelView _buildingPanel;
    
    public override void OnUpdate()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        if (!EventSystem.current.IsPointerOverGameObject())
            TileSelector.Instance.OnMouseClick();
    }

    public void ChangeDialoguePanelVisibility()
    {
        dialoguePanel.SetActive(!dialoguePanel.activeSelf);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        _buildingPanel.Open();
    }
    public override void OnExit() //similar to base.OnExit but don't hide it
    {
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
    }
}
