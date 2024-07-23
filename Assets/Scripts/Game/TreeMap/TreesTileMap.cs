using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TreesTileMap : TreeMap
{
    private Tilemap _treesTileMap;
    public override bool GetValueAtPosition(int x, int y)
    {
        if(_treesTileMap == null)
            _treesTileMap = GetComponent<Tilemap>();
        return _treesTileMap.HasTile(new Vector3Int(x, y));
    }
}
