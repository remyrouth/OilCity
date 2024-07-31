
namespace Game.Tutorial
{
    public class TutorialFirstStep : TutorialStep
    {
        private new void OnEnable()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            base.OnEnable();
            TimeManager.Instance.TicksPerMinute = 0;
            TileSelector.Instance.SelectorEnabled = false;
            DialogueUI.Instance.EnableDialogue();
        }

        private void OnDisable()
        {
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
        }
    }
}
