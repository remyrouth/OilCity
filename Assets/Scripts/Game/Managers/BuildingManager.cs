using System;
using System.Collections;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager>
{
    private BuildingPreview _currentPreview;
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
        _currentPreview = Instantiate(so.previewPrefab).GetComponent<BuildingPreview>();
        _currentPreview.InitSO(so);

        _coroutine = StartCoroutine(IEDoBuildingProcess());
    }

    public void CancelBuilding()
    {
        Destroy(_currentPreview);
        _currentPreview = null;
        _currentBuildingSO = null;
        _coroutine = null;
    }

    private IEnumerator IEDoBuildingProcess()
    {
        yield return _currentPreview.IEDoBuildProcess();

        CancelBuilding();
    }

}
