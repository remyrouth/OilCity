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

    public void StartFlicker(int buttonIndex)
    {
        Debug.Log($"StartFlciker called for button index {buttonIndex}");
        var buildingButton = buildingButtons[buttonIndex].GetComponent<BuildingButton>();
        if (buildingButton != null)
        {
            buildingButton.StartFlicker();
        }
        else
        {
            Debug.LogError("BuildingButton component not found on the button.");
        }
    }

    public void StopFlicker(int buttonIndex)
    {
        Debug.Log($"StopFlicker called for button index {buttonIndex}");
        var buildingButton = buildingButtons[buttonIndex].GetComponent<BuildingButton>();
        if (buildingButton != null)
        {
            buildingButton.StopFlicker();
        }
        else
        {
            Debug.LogError("BuildingButton component not found on the button.");
        }
    }
}
