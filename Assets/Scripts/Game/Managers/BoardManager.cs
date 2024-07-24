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
    [SerializeField] private Transform _treeHolder;

    public Dictionary<Vector2Int, TileObjectController> tileDictionary { get; private set; } = new();

    private void Start()
    {
        for (int i = 0; i < MAP_SIZE_X; i++)
        {
            for (int j = 0; j < MAP_SIZE_Y; j++)
            {
                if (!TreeEvaluator.GetValueAtPosition(i, j))
                    continue;
                var obj = Instantiate(_treePrefab, _treeHolder);
                obj.transform.position = new Vector3(i, j, 0);
                tileDictionary.Add(new Vector2Int(i, j), obj.GetComponent<TreeController>());
            }
        }
    }

    public bool Create(Vector2Int position, BuildingScriptableObject buildingSO)
    {
        if (AreTilesOccupiedForBuilding(position, buildingSO))
            return false;
        var obj = buildingSO.CreateInstance();
        obj.transform.position = new Vector3(position.x, position.y, 0);
        for (int i = 0; i < buildingSO.size.y; i++)
            for (int j = 0; j < buildingSO.size.x; j++)
                tileDictionary[position + new Vector2Int(j, i)] = obj;

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
    /// Gets tiles in range of a TileObjectController
    /// </summary>
    /// <param name="building"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public List<Vector2Int> GetTilesInRange(TileObjectController building, int range)
    {
        Vector2Int upperRight = building.Anchor + building.size;
        List<Vector2Int> tiles = new();

        for (int x = building.Anchor.x - range; x < upperRight.x + range; x++)
        {
            for (int y = building.Anchor.y - range; y < upperRight.y + range; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                if (IsPositionOutsideBoard(currentPos))
                    continue;
                int xDistance = Mathf.Max(0, building.Anchor.x - currentPos.x, currentPos.x - upperRight.x);
                int yDistance = Mathf.Max(0, building.Anchor.y - currentPos.y, currentPos.y - upperRight.y);

                if (new Vector2Int(xDistance, yDistance).sqrMagnitude <= range * range)
                    tiles.Add(currentPos);
            }
        }

        return tiles;
    }

    /// <summary>
    /// Checks if a given position is outside of the board
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsPositionOutsideBoard(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= MAP_SIZE_X)
            return true;
        if (pos.y < 0 || pos.y >= MAP_SIZE_Y)
            return true;
        return false;
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
        var tiles = tileDictionary.Where(pos => pos.Value == tileObject).Select(pos => pos.Key).ToList();
        foreach (Vector2Int position in tiles)
            Destroy(position);
        Destroy(tileObject.gameObject);
    }
    private void Destroy(Vector2Int position)
    {
        if (!IsTileOccupied(position)) return;
        tileDictionary.Remove(position);
    }
}
