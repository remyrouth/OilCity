using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelUI : Singleton<BuildingPanelUI>
{
    [SerializeField] private List<Button> buildingButtons;

    public void EnableAllButtons()
    {
        foreach (var button in buildingButtons)
        {
            button.interactable = true;
        }
    }
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
    
    public void ToggleButtonInteractableWithHighlight(int buttonIndex)
    {
        buildingButtons[buttonIndex].interactable = !buildingButtons[buttonIndex].interactable;
        ToggleHighlight(buttonIndex);
    }

    public Transform GetButtonTransform(int index)
    {
        return buildingButtons[index].transform;
    }

    public void ToggleHighlight(int buttonIndex)
    {
        Debug.Log($"StartFlciker called for button index {buttonIndex}");
        var buildingButton = buildingButtons[buttonIndex].GetComponent<BuildingButton>();
        if (buildingButton != null)
        {
            buildingButton.ToggleHighlight();
        }
        else
        {
            Debug.LogError("BuildingButton component not found on the button.");
        }
    }
}
