using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : UIState
{
    public override GameState type => GameState.GameUI;

    [SerializeField] private GameObject dialoguePanel;
    
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
}
