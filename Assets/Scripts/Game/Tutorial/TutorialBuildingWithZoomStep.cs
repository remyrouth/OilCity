using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialBuildingWithZoomStep : TutorialStep
    {
        [SerializeField] private string buildingName;
        [SerializeField] private int buttonToUnlockIndex;
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float targetZoom;
        

        private new void OnEnable()
        {
            BoardManager.Instance.OnBuildingPlaced += FinishStep;
            base.OnEnable();
            CameraController.Instance.TargetPosition = targetPosition;
            CameraController.Instance.TargetZoom = targetZoom;
            BuildingPanelUI.Instance.ToggleButtonInteractableWithHighlight(buttonToUnlockIndex);
        }

        private void OnDisable()
        {
            BoardManager.Instance.OnBuildingPlaced -= FinishStep;
        }
    
        private void FinishStep(Vector2Int position, BuildingScriptableObject buildingSO)
        {
            if (buildingSO.name.Equals(buildingName))
            {
                BuildingPanelUI.Instance.ToggleHighlight(buttonToUnlockIndex);
                FinishStep();
            }
        }
    }
}