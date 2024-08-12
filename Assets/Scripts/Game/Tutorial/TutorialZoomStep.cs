using System;
using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialZoomStep : TutorialStep
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float targetZoom;

        public override void Initialize()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            DialogueUI.Instance.ToggleIndicator();
            base.Initialize();
        }

        public void LateUpdate()
        {
            CameraController.Instance.TargetPosition = targetPosition;
            CameraController.Instance.TargetZoom = targetZoom;
        }

        public override void Deinitialize()
        {
            DialogueUI.Instance.ToggleIndicator();
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
        }
    }
 
}
