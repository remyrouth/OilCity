using System;
using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(Tilemap))]
public class OilTileMapController : OilMapController
{
    private Tilemap _oilTileMap;
    private void Awake() => gameObject.SetActive(false);
    protected override float GetBaseValue(int x, int y)
    {
        if (_oilTileMap == null)
            _oilTileMap = GetComponent<Tilemap>();
        try
        {
            return _oilTileMap.GetColor(new Vector3Int(x, y)).g;
        } catch (Exception)
        {
            return 0;
        }
    }
}
