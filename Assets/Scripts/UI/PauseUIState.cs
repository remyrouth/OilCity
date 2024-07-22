namespace UI
{
    public class PauseUIState: UIState
    {
        public override GameState GetGameState()
        {
            return GameState.PauseUI;
        }
    }
}