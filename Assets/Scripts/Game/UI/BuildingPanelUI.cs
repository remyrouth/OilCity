using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelUI : Singleton<BuildingPanelUI>
{
    [SerializeField] private List<Button> buildingButtons;
    /// <summary>
    /// ref for flicker
    /// 
    /// method for given index
    /// 
    /// moves flicker to highlighted button
    /// 
    /// disable when tutorial done
    /// </summary>
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
