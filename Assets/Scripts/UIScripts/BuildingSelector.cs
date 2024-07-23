using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    
    private string _selectedBuilding;
    
    public void SelectBuilding(string buildingType)
    {
        _selectedBuilding = buildingType;
        Debug.Log("Selected building: " + _selectedBuilding);
    }
}
