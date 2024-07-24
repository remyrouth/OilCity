using System;
using System.Collections;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager>
{
    private IPlacer _currentPlacer;
    private BuildingScriptableObject _currentBuildingSO;

    private Coroutine _coroutine;

    public void BeginBuilding(BuildingScriptableObject so)
    {
        if (_coroutine != null)
        {
            Debug.LogError("Already placing another object! Canceling...");
            return;
        }

        _currentBuildingSO = so;
        _currentPlacer = Instantiate(so.previewPrefab).GetComponent<BuildingPlacer>();
        _currentPlacer.InitSO(so);

        _coroutine = StartCoroutine(IEDoBuildingProcess());
    }

    public void CancelBuilding()
    {
        _currentPlacer.Cleanup();
        _currentPlacer = null;

        _currentBuildingSO = null;
        _coroutine = null;
    }

    private IEnumerator IEDoBuildingProcess()
    {
        yield return _currentPlacer.IEDoBuildProcess();

        CancelBuilding();
    }

}
