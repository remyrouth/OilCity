using System;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager>
{
    private BuildingScriptableObject _currentConfig;
    private BuildingPreview _currentPreview;
    private Action OnClicked;
    private void Update()
    {
        if (_currentPreview != null)
        {
            var mousePos = TileSelector.Instance.MouseToGrid();
            _currentPreview.transform.position = new Vector3(mousePos.x,mousePos.y,0);
            var canBuild = BoardManager.Instance.AreTilesOccupiedForBuilding(mousePos, _currentConfig);
            _currentPreview.SetState(canBuild);
        }
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
    }

    public void BeginBuilding(BuildingScriptableObject SO)
    {
        CancelBuilding();
        _currentConfig = SO;
        _currentPreview = Instantiate(SO.previewPrefab).GetComponent<BuildingPreview>();
        OnClicked = PlaceIfCan;
    }
    private void PlaceIfCan()
    {
        Vector2Int pos = TileSelector.Instance.MouseToGrid();
        if(_currentConfig == null || _currentPreview == null)
            return;
        if (BoardManager.Instance.AreTilesOccupiedForBuilding(pos, _currentConfig))
            _currentPreview.SetState(false);

        _currentConfig.CreateInstance().transform.position = new Vector3(pos.x,pos.y,0);

        CancelBuilding();
    }
    public void BeginBuildingPipe(PipeScriptableObject SO)
    {

    }

    public void CancelBuilding()
    {
        Destroy(_currentPreview);
        _currentConfig = null;
        _currentPreview = null;
        OnClicked = null;
    }

}
