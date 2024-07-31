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
            base.OnEnable();
            _canContinue = false;
            TimeManager.Instance.TicksPerMinute = 60;
            StartCoroutine(Wait());
        }

        private IEnumerator Wait()
        {
            CameraController.Instance.TargetPosition = targetPosition;
            CameraController.Instance.TargetZoom = targetZoom;
            yield return new WaitForSeconds(5);
            _canContinue = true;
            TimeManager.Instance.TicksPerMinute = 0;
        }

        private void Update()
        {
            if (_canContinue && Input.GetMouseButtonDown(0))
            {
                FinishStep();
            }
        }
    }
}