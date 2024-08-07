using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialBuildingStep : TutorialStep
    {
        [SerializeField] private string buildingName;
        [SerializeField] private int buttonToUnlockIndex;
        
        public override void Initialize()
        {
            base.Initialize();
            BoardManager.Instance.OnBuildingPlaced += FinishStep;
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
                BuildingPanelUI.Instance.ToggleHighlight(buttonToUnlockIndex);
                FinishStep();
            }
        }
    }

}
