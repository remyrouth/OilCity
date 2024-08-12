using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialBuildingWithZoomStep : TutorialStep
    {
        [SerializeField] private string buildingName;
        [SerializeField] private int buttonToUnlockIndex;
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float targetZoom;
        

        public override void Initialize()
        {
            BoardManager.Instance.OnBuildingPlaced += FinishStep;
            base.Initialize();
            DialogueUI.Instance.EnableArrow(buildingName);
            CameraController.Instance.TargetPosition = targetPosition;
            CameraController.Instance.TargetZoom = targetZoom;
            BuildingPanelUI.Instance.ToggleButtonInteractableWithHighlight(buttonToUnlockIndex);
        }

        public override void Deinitialize()
        {
            BoardManager.Instance.OnBuildingPlaced -= FinishStep;
        }
    
        private void FinishStep(Vector2Int position, BuildingScriptableObject buildingSO)
        {
            if (buildingSO.name.Equals(buildingName))
            {
                DialogueUI.Instance.DisableArrow();
                BuildingPanelUI.Instance.ToggleHighlight(buttonToUnlockIndex);
                FinishStep();
            }
        }
    }
}