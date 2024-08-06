using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialHighlightStep : TutorialStep
    {
        [SerializeField] private int highlightIndexToToggle;
        
        private new void OnEnable()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            base.OnEnable();
            HighlightUI.Instance.ToggleHighlight(highlightIndexToToggle);
        }

        private void OnDisable()
        {
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
            HighlightUI.Instance.ToggleHighlight(highlightIndexToToggle);
            
        }
    }
}