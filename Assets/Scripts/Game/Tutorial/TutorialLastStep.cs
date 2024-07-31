using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialLastStep : TutorialStep
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
    
        private new void FinishStep()
        {
            DialogueUI.Instance.DisableDialogue();
            BuildingPanelUI.Instance.EnableAllButtons();
            TimeManager.Instance.TicksPerMinute = 60;
            TileSelector.Instance.SelectorEnabled = true;
            TutorialManager.Instance.InTutorial = false;

            Destroy(gameObject);
        }
    }

}
