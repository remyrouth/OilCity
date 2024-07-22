using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BoardManager : Singleton<BoardManager>
{
    public Dictionary<Vector2Int, TileObjectController> tileDictionary { get; private set; }
    public void Create(Vector2Int position, BuildingScriptableObject buildingSO)
    {
        AreTilesOccupiedForBuilding(position, buildingSO);
        var obj = buildingSO.CreateInstance();
        for (int i = 0; i < buildingSO.size.y; i++)
        {
            for (int j = 0; j < buildingSO.size.x; j++)
            {
                tileDictionary[position + new Vector2Int(j,i)] = obj;
            }
        }
    }
    public bool IsTileOccupied(Vector2Int position)
    {
        return tileDictionary.ContainsKey(position);
    }
    public bool IsTileOccupied(List<Vector2Int> listOfPositions)
    {
        foreach (Vector2Int position in listOfPositions)
        {
            if (tileDictionary.ContainsKey(position)) return true;
        }
        return false;
    }
    public bool AreTilesOccupiedForBuilding(Vector2Int position, BuildingScriptableObject buildingSO)
    {
        
        for (int i = 0; i < buildingSO.size.y; i++)
        {
            for (int j = 0; j < buildingSO.size.x; j++)
            {
                if (IsTileOccupied(position + new Vector2Int(j,i))) return true;
            }
        }
        return false;
    }
    public void Destroy(TileObjectController tileObject)
    {
        foreach (Vector2Int position in tileDictionary.Where(pos => pos.Value == tileObject).Select(pos => pos.Key))
        {
            Destroy(position);
        }
    }
    private void Destroy(Vector2Int position)
    {
        if (!IsTileOccupied(position)) return;
        tileDictionary.Remove(position);
    }
}
