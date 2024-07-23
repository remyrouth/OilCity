using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager>
{


    private BuildingScriptableObject _currentPreview;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
    }

    public void BeginBuilding(BuildingScriptableObject SO)
    {

    }
    public void BeginBuildingPipe(PipeScriptableObject SO)
    {

    }

    public void CancelBuilding()
    {

    }

}
