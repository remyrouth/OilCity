using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialHighlightStep : TutorialStep
    {
        [SerializeField] private int highlightIndexToToggle;

        public override void Initialize()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            base.Initialize();
            HighlightUI.Instance.ToggleHighlight(highlightIndexToToggle);
        }

        public override void Deinitialize()
        {
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
            HighlightUI.Instance.ToggleHighlight(highlightIndexToToggle);
            
        }
    }
}