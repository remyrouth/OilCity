using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BuildingManager : Singleton<BuildingManager>
{
    private IPlacer _currentPlacer;
    private BuildingScriptableObject _currentBuildingSO;

    private Coroutine _coroutine;

/*
    public void BeginBuilding(BuildingScriptableObject so)
    {
        if (_coroutine != null)
        {
            var mousePos = TileSelector.Instance.MouseToGrid();
            _currentPreview.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
            var canBuild = BoardManager.Instance.AreTilesOccupiedForBuilding(mousePos, _currentConfig);
            _currentPreview.SetState(canBuild);
        }
    }*/

    public void OnMouseClick()=>_currentPlacer?.PressMouse();

    public void BeginBuilding(BuildingScriptableObject SO)
    {
        CancelBuilding();
        UIStateMachine.Instance.ChangeState(GameState.BuildingUI);
        _currentBuildingSO = SO;
        
        _currentPlacer = Instantiate(SO.previewPrefab).GetComponent<BuildingPlacer>();
        _currentPlacer.InitSO(SO);

        _coroutine = StartCoroutine(IEDoBuildingProcess());

        //OnClicked = PlaceIfCan;
    }
    /*
    private void PlaceIfCan()
    {
        Vector2Int pos = TileSelector.Instance.MouseToGrid();
        if (_currentConfig == null || _currentPreview == null)
            return;
        }

        if (BoardManager.Instance.Create(pos, _currentConfig))
            CancelBuilding();
    }
    public void BeginBuildingPipe(PipeScriptableObject SO)
    {
        _currentBuildingSO = so;
        _currentPlacer = Instantiate(so.previewPrefab).GetComponent<BuildingPlacer>();
        _currentPlacer.InitSO(so);

        _coroutine = StartCoroutine(IEDoBuildingProcess());
    }*/

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