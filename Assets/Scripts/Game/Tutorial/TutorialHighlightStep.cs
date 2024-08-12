using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialHighlightStep : TutorialStep
    {
        [SerializeField] private int highlightIndexToToggle;
        [SerializeField] private string animationName;

        public override void Initialize()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            DialogueUI.Instance.EnableArrow(animationName);
            DialogueUI.Instance.ToggleIndicator();
            base.Initialize();
            HighlightUI.Instance.ToggleHighlight(highlightIndexToToggle);
        }

        public override void Deinitialize()
        {
            DialogueUI.Instance.ToggleIndicator();
            DialogueUI.Instance.DisableArrow();
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
            HighlightUI.Instance.ToggleHighlight(highlightIndexToToggle);
        }
    }
}