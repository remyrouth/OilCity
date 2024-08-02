using System.Collections.Generic;
using UnityEngine;

public class TileSelector : Singleton<TileSelector>
{
    [SerializeField] private bool selectorEnabled;

    public bool SelectorEnabled
    {
        get => selectorEnabled;
        set => selectorEnabled = value;
    }

    /// <summary>
    /// Returns the mouses position in the grid
    /// </summary>
    /// <returns></returns>
    public Vector2Int MouseToGrid()
    {
        Vector3 mouseCursorPosition = Camera.main.ScreenToWorldPoint(ControlManager.Instance.RetrieveMousePosition());
        Vector2Int cursorCellPosition = new Vector2Int((int)mouseCursorPosition.x, (int)mouseCursorPosition.y);
        return cursorCellPosition;

        // Vector3 mouseSpotPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector2Int mouseCellPosition = new Vector2Int((int)mouseSpotPosition.x, (int)mouseSpotPosition.y);
        // return mouseCellPosition;
    }
    public void OnMouseClick()
    {
        if (!selectorEnabled || !BoardManager.Instance.IsTileOccupied(MouseToGrid()))
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
    private List<GameObject> views = new();
    private void BeginFocus(TileObjectController toc)
    {
        EndFocus();
        _currentSelected = toc;
        bool fanUpsideDown = toc.ActionsPivot.y > BoardManager.MAP_SIZE_Y - 3;
        if (fanUpsideDown)
            _actionsCanvas.transform.position
                = new Vector3(_currentSelected.ActionsPivot.x, _currentSelected.Anchor.y, 0);
        else
            _actionsCanvas.transform.position = _currentSelected.ActionsPivot;

        var actions = _currentSelected.GetActions();
        if (actions.Count == 0)
            EndFocus();
        ShowActionFan(actions, fanUpsideDown);
    }
    private void ShowActionFan(List<TileAction> actions, bool upsideDown)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            float angle;
            if (upsideDown)
                angle = -105 - i * 50;
            else
                angle = -75 + i * 50;

            var visual = actions[i].Create(_actionsCanvas.transform
                , angle, _currentSelected, this, upsideDown);
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
