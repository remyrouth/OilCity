using UnityEngine;

public class TutorialStepSeven : TutorialStep
{
    [SerializeField] private int buttonToUnlockIndex;
    
    private new void OnEnable()
    {
        BoardManager.Instance.OnBuildingPlaced += FinishStep;
        base.OnEnable();
        BuildingPanelUI.Instance.ToggleButtonInteractable(buttonToUnlockIndex);
    }
    
    private void OnDisable()
    {
        BoardManager.Instance.OnBuildingPlaced -= FinishStep;
    }
    
    private void FinishStep(Vector2Int position, BuildingScriptableObject buildingSO)
    {
        if (buildingSO.name.Equals("WoodCutter"))
        {
            FinishStep();
        }
    }
}