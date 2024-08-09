
public class EventUI : UIState
{
    public override GameState type => GameState.EventUI;
    
    public override void OnEnter()
    {
        base.OnEnter();
        TimeManager.Instance.TicksPerMinute = 0;
        BuildingManager.Instance.CancelBuilding();
    }

    public override void OnExit()
    {
        base.OnExit();
        TimeManager.Instance.TicksPerMinute = 60;
    }
}