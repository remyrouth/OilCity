using System.Collections.Generic;
using UnityEngine;

public class TileSelector : Singleton<TileSelector>
{

    public Vector2Int MouseToGrid()
    {
        Vector3 mouseSpotPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int mouseCellPosition = new Vector2Int(Mathf.CeilToInt(mouseSpotPosition.x), Mathf.CeilToInt(mouseSpotPosition.y));
        return mouseCellPosition;
    }
    public void OnMouseClick()
    {
        if (!BoardManager.Instance.IsTileOccupied(MouseToGrid()))
        {
            EndFocus();
            return;
        }
        if (BoardManager.Instance.tileDictionary[MouseToGrid()].TryGetComponent(out TileObjectController building))
            BeginFocus(building);
        else
            EndFocus();
    }

    [SerializeField] private GameObject _actionPrefab;
    [SerializeField] private Canvas _actionsCanvas;
    private TileObjectController _currentSelected;
    private List<SingleTileActionView> views = new();
    private void BeginFocus(TileObjectController toc)
    {
        EndFocus();
        _currentSelected = toc;
        _actionsCanvas.transform.position = _currentSelected.ActionsPivot;

        var actions = _currentSelected.GetActions();
        if (actions.Count == 0)
            EndFocus();

        for (int i = 0; i < actions.Count; i++)
        {
            var visual = Instantiate(_actionPrefab, _actionsCanvas.transform)
                .GetComponent<SingleTileActionView>();
            visual.Initialize(actions[i], i * 180 / actions.Count, _currentSelected, this);
            views.Add(visual);
        }
    }
    public void EndFocus()
    {
        if (views.Count > 0)
        {
            for (int i = 0; i < views.Count; i++)
                Destroy(views[i].gameObject);
            views.Clear();
        }
        _currentSelected = null;
    }


}
