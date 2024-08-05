using System.Collections.Generic;
using UnityEngine;

public class BuildingPreviewRangeShower : TileObjectController
{
    private AOEBuildingScriptableObject _so;
    public override Vector2Int size => _so.size;


    [SerializeField] private GameObject _tilePrefab;
    private List<GameObject> _visibleTiles = new();

    public void ShowRadius(AOEBuildingScriptableObject config)
    {
        if (_so == null)
            _so = config;
        var tilesInRange = BoardManager.Instance.GetTilesInRange(this, config.BaseRange);
        foreach (var tile in tilesInRange)
        {
            var obj = Instantiate(_tilePrefab, tile.ToVector3() + Vector3.one / 2, Quaternion.identity);
            obj.transform.SetParent(transform, true);
            _visibleTiles.Add(obj);
            obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.25f);
            obj.GetComponent<SpriteRenderer>().sortingOrder = -4;
        }
    }
    public void HideRadius()
    {
        foreach (var tile in _visibleTiles)
            Destroy(tile);
        _visibleTiles.Clear();
    }
}
