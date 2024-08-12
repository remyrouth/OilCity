
namespace Game.Tutorial
{
    public class TutorialFirstStep : TutorialStep
    {
        public override void Initialize()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            DialogueUI.Instance.EnableDialogue();
            DialogueUI.Instance.ToggleIndicator();
            base.Initialize();
            TimeManager.Instance.TicksPerMinute = 0;
            TileSelector.Instance.SelectorEnabled = false;
        }

        public override void Deinitialize()
        {
            DialogueUI.Instance.ToggleIndicator();
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
        }
    }
}
