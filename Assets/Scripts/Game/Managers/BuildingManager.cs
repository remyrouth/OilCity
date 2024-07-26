using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BuildingManager : Singleton<BuildingManager>
{
    private IPlacer _currentPlacer;
    private BuildingScriptableObject _currentBuildingSO;

    private Coroutine _coroutine;

    public void OnMouseClick()=>_currentPlacer?.PressMouse();

    public void BeginBuilding(BuildingScriptableObject SO)
    {
        CancelBuilding();
        UIStateMachine.Instance.ChangeState(GameState.BuildingUI);
        _currentBuildingSO = SO;
        
        _currentPlacer = Instantiate(SO.previewPrefab).GetComponent<BuildingPlacer>();
        _currentPlacer.InitSO(SO);

        _coroutine = StartCoroutine(IEDoBuildingProcess());
    }

    public void CancelBuilding()
    {
        _currentPlacer?.Cleanup();
        _currentPlacer = null;

        _currentBuildingSO = null;
        _coroutine = null;
        
        UIStateMachine.Instance.ChangeState(GameState.GameUI);
    }

    private IEnumerator IEDoBuildingProcess()
    {
        yield return _currentPlacer.IEDoBuildProcess();

        CancelBuilding();
    }
}