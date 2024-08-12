using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialHighlightWithZoomStep : TutorialStep
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float targetZoom;
        [SerializeField] private int highlightIndexToToggle;
        [SerializeField] private string animationName;

        public override void Initialize()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            DialogueUI.Instance.EnableArrow(animationName);
            base.Initialize();
            CameraController.Instance.TargetPosition = targetPosition;
            CameraController.Instance.TargetZoom = targetZoom;
            HighlightUI.Instance.ToggleHighlight(highlightIndexToToggle);
            DialogueUI.Instance.ToggleIndicator();
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