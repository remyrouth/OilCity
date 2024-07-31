using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialZoomStep : TutorialStep
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float targetZoom;
        
        private new void OnEnable()
        {
            base.OnEnable();
            CameraController.Instance.TargetPosition = targetPosition;
            CameraController.Instance.TargetZoom = targetZoom;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                FinishStep();
            }
        }
    }
 
}
