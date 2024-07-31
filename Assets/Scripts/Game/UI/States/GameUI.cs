using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : UIState
{
    public override GameState type => GameState.GameUI;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private BuildingPanelView _buildingPanel;

    private ControlManager controlManager;
    
    public override void OnUpdate()
    {

    }

    private void Start() {
        controlManager = FindObjectOfType<ControlManager>();
        if (controlManager != null) {
            controlManager.leftClickActivationButtontrigger += MouseClick;
        } else {
            Debug.LogError("ControlManager not found in the scene.");
        }
    }

    private void MouseClick() {
        // Debug.Log("Mouse Clicked from ControlManager delegate");
        if (UIStateMachine.Instance.CurrentStateType != type)
            return;
        if (!EventSystem.current.IsPointerOverGameObject()) {
            TileSelector.Instance.OnMouseClick();
        }
    }


    private void OnDestroy() {
        if (controlManager != null) {
            controlManager.leftClickActivationButtontrigger -= MouseClick;
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
    }
    public override void OnExit() //similar to base.OnExit but don't hide it
    {
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
    }
}
