using System.Collections.Generic;
using UnityEngine;

public class BuildingRangeShower : MonoBehaviour
{
    [SerializeField] private GameObject _tilePrefab;
    private List<GameObject> _visibleTiles = new();
    public void ShowRadius()
    {
        var controller = GetComponent<AOEBuildingController>();
        var tilesInRange = BoardManager.Instance.GetTilesInRange(controller, controller.Range);
        foreach (var tile in tilesInRange)
        {
            var obj = Instantiate(_tilePrefab, tile.ToVector3() + Vector3.one / 2, Quaternion.identity);
            obj.transform.SetParent(transform, true);
            _visibleTiles.Add(obj);
            obj.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.25f);
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
