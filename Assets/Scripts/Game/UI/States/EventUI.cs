
public class EventUI : UIState
{
    public override GameState type => GameState.EventUI;

    private void OnEnable()
    {
        ControlManager.Instance.leftClickActivationButtontrigger += MouseClick;
    }
    
    private void OnDisable()
    {
        ControlManager.Instance.leftClickActivationButtontrigger -= MouseClick;
    }
    private void MouseClick()
    {
        if (UIStateMachine.Instance.CurrentStateType != type)
            return;
        UIStateMachine.Instance.ChangeState(GameState.GameUI);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        TimeManager.Instance.TicksPerMinute = 0;
    }

    public override void OnExit()
    {
        base.OnExit();
        TimeManager.Instance.TicksPerMinute = 60;
    }
}