public class IngameMenuUI : UIState
{
    public override GameState type => GameState.MenuUI;
    public override void OnEnter() => TimeManager.Instance.TicksPerMinute = 0;
    public override void OnExit() => TimeManager.Instance.TicksPerMinute = 60;
}
