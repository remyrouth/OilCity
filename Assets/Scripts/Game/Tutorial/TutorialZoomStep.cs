using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialZoomStep : TutorialStep
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float targetZoom;
        
        private new void OnEnable()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            base.OnEnable();
            CameraController.Instance.TargetPosition = targetPosition;
            CameraController.Instance.TargetZoom = targetZoom;
        }

        private void OnDisable()
        {
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
        }
    }
 
}
