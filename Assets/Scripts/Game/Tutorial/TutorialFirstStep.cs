
namespace Game.Tutorial
{
    public class TutorialFirstStep : TutorialStep
    {
        private new void OnEnable()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            DialogueUI.Instance.EnableDialogue();
            base.OnEnable();
            TimeManager.Instance.TicksPerMinute = 0;
            TileSelector.Instance.SelectorEnabled = false;
        }

        private void OnDisable()
        {
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
        }
    }
}
