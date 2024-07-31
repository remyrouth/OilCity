using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialDialogueStep : TutorialStep
    {
        private new void OnEnable()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            base.OnEnable();
        }

        private void OnDisable()
        {
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
        }
    }
}
