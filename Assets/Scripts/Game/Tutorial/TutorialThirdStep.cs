using UnityEngine;

public class TutorialThirdStep : TutorialStep
{
    [SerializeField] private int buttonToUnlockIndex;
    private new void OnEnable()
    {
        //enable flicker
        //

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
        if (buildingSO.name.Equals("Refinery"))
        {
            //disable when done with third step

            FinishStep();
        }
    }
}
