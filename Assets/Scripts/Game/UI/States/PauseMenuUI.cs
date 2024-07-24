using UnityEngine;

public class PauseMenuUI : UIState
{
    public override GameState type => GameState.PauseUI;
    [SerializeField] private GameObject[] panels;
    private GameObject _currentPanel;

    public override void OnEnter()
    {
        base.OnEnter();
        TimeManager.Instance.TicksPerMinute = 0;
        _currentPanel = panels[0];
    }

    public override void OnExit()
    {
        base.OnExit();
        TimeManager.Instance.TicksPerMinute = 60;
    }

    public void ChangePanel(int panelIndex)
    {
        _currentPanel.SetActive(false);
        _currentPanel = panels[panelIndex];
        _currentPanel.SetActive(true);
    }
}
