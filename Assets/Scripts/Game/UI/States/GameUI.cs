using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : UIState
{
    public override GameState type => GameState.GameUI;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private BuildingPanelView _buildingPanel;

    public override void OnUpdate()
    {

    }

    private void Start()
    {
        ControlManager.Instance.leftClickActivationButtontrigger += LeftMouseClick;
    }
    private void OnDestroy()
    {
        if (ControlManager.Instance != null)
            ControlManager.Instance.leftClickActivationButtontrigger -= LeftMouseClick;
    }

    private void LeftMouseClick()
    {
        if (UIStateMachine.Instance.CurrentStateType != type)
        {
            return;
        }
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            TileSelector.Instance.OnMouseClick();
        }
    }

    public void ChangeDialoguePanelVisibility()
    {
        dialoguePanel.SetActive(!dialoguePanel.activeSelf);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        _buildingPanel.Open();
        if (!TutorialManager.Instance.InTutorial)
        {
            TimeManager.Instance.TicksPerMinute = 60;
            BuildingPanelUI.Instance.EnableAllButtons();
        }
    }
    public override void OnExit() //similar to base.OnExit but don't hide it
    {
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
    }
}
