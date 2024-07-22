
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OilTileMapController : OilMapController
{
    private Tilemap oilTileMap;


    protected override float GetBaseValue(int x, int y)
    {
        try
        {
            return oilTileMap.GetColor(new Vector3Int(x, y)).g / 255;
        } catch (Exception)
        {
            return 0;
        }
    }
}
