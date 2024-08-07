using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialHighlightWithZoomStep : TutorialStep
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float targetZoom;
        [SerializeField] private int highlightIndexToToggle;

        public override void Initialize()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            base.Initialize();
            CameraController.Instance.TargetPosition = targetPosition;
            CameraController.Instance.TargetZoom = targetZoom;
            HighlightUI.Instance.ToggleHighlight(highlightIndexToToggle);
        }

        public override void Deinitialize()
        {
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
            HighlightUI.Instance.ToggleHighlight(highlightIndexToToggle);
        }
    }
}