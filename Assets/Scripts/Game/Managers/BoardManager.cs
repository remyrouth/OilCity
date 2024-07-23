using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BoardManager : Singleton<BoardManager>
{
    public const int MAP_SIZE_X = 40;
    public const int MAP_SIZE_Y = 40;
    [field: SerializeField] public OilMapController OilEvaluator { get; private set; }

    [field: SerializeField] public TreeMap TreeEvaluator { get; private set; }
    [SerializeField] private GameObject _treePrefab;

    public Dictionary<Vector2Int, TileObjectController> tileDictionary { get; private set; }

    private void Start()
    {
        for (int i = 0; i < MAP_SIZE_X; i++)
            for (int j = 0; j < MAP_SIZE_Y; j++)
                if (TreeEvaluator.GetValueAtPosition(i, j))
                    tileDictionary.Add(new Vector2Int(i, j)
                        , Instantiate(_treePrefab, new Vector3(i, j, 0), Quaternion.identity)
                        .GetComponent<TreeController>());
    }

    public bool Create(Vector2Int position, BuildingScriptableObject buildingSO)
    {
        if (AreTilesOccupiedForBuilding(position, buildingSO))
            return false;
        var obj = buildingSO.CreateInstance();
        for (int i = 0; i < buildingSO.size.y; i++)
        {
            for (int j = 0; j < buildingSO.size.x; j++)
            {
                tileDictionary[position + new Vector2Int(j, i)] = obj;
            }
        }
        return true;
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
                if (IsTileOccupied(position + new Vector2Int(j, i))) return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns a list of all the tile objects that are adjacent to the sides of the building
    /// </summary>
    /// <param name="position"></param>
    /// <param name="buildingSO"></param>
    /// <returns></returns>
    public List<TileObjectController> GetPeripheralTileObjectsForBuilding(Vector2Int position, Vector2Int size)
    {
        var list = new List<TileObjectController>();

        int top = size.y;
        int right = size.x;

        for (int i = 0; i <= top; i++)
        {
            for (int j = 0; j <= right; j++)
            {
                // for the sake of readability
                bool is_topleft_corner = (i == top && j == 0);
                bool is_topright_corner = (i == top && j == right);
                bool is_bottomleft_corner = (i == 0 && j == 0);
                bool is_bottomright_corner = (i == 0 && j == right);

                var offset_position = position + new Vector2Int(j, i);

                if (is_topleft_corner || is_topright_corner || is_bottomleft_corner || is_bottomright_corner) continue;
                else if (tileDictionary.TryGetValue(offset_position, out var value))
                {
                    list.Add(value);
                }
            }
        }

        return list;
    }

    /// <summary>
    /// Converts the current world position of the Vector3 to the grid space
    /// </summary>
    /// <returns></returns>
    public static Vector2Int ConvertWorldspaceToGrid(Vector3 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
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
