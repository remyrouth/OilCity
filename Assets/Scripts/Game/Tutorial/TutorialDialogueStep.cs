using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialDialogueStep : TutorialStep
    {
        public override void Initialize()
        {
            DialogueUI.Instance.ToggleIndicator();
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            base.Initialize();
        }

        public override void Deinitialize()
        {
            DialogueUI.Instance.ToggleIndicator();
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
        }
    }
}
