using System.Collections;
using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialTownSpawnTutorial : TutorialStep
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float targetZoom;
        private bool _canContinue;
        private new void OnEnable()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            base.OnEnable();
            _canContinue = false;
            TimeManager.Instance.TicksPerMinute = 60;
            StartCoroutine(Wait());
        }
        
        private void OnDisable()
        {
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
        }
        
        private IEnumerator Wait()
        {
            CameraController.Instance.TargetPosition = targetPosition;
            CameraController.Instance.TargetZoom = targetZoom;
            yield return new WaitForSeconds(5);
            _canContinue = true;
            TimeManager.Instance.TicksPerMinute = 0;
        }

        private new void FinishStep()
        {
            if (_canContinue)
            {
                base.FinishStep();
            }
        }
    }
}