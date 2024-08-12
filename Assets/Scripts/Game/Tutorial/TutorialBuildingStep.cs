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
            DialogueUI.Instance.DisableArrow();
            DialogueUI.Instance.EnableArrow(buildingName);
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
