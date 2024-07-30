using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelUI : Singleton<BuildingPanelUI>
{
    [SerializeField] private List<Button> buildingButtons;

    public void DisableAllButtons()
    {
        foreach (var button in buildingButtons)
        {
            button.interactable = false;
        }
    }

    public void ToggleButtonInteractable(int buttonIndex)
    {
        buildingButtons[buttonIndex].interactable = !buildingButtons[buttonIndex].interactable;
    }
    
}
