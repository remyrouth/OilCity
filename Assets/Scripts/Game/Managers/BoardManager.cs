using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : Singleton<BoardManager>
{
    public const int MAP_SIZE_X = 40;
    public const int MAP_SIZE_Y = 40;
    [field: SerializeField] public OilMapController OilEvaluator { get; private set; }
    [field: SerializeField] public TreeMap TreeEvaluator { get; private set; }
    [SerializeField] private GameObject _treePrefab;
    [SerializeField] private Transform _treeHolder;
    [SerializeField] private Tilemap _pipeTileMap;

    [System.Serializable]
    private struct InitialBuilding
    { public BuildingScriptableObject config; public Vector2Int pos; }
    [SerializeField] private InitialBuilding[] InitialBuildings;
    public Dictionary<Vector2Int, TileObjectController> tileDictionary { get; private set; } = new();
    /// <summary>
    /// Generates the forest and the initial buildings
    /// </summary>
    private void Start()
    {
        if (_pipeTileMap == null) {
            Debug.LogError("You did not attach the pipe tilemap to the board manager in the inspector. This must be done before the game starts");
        }

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
        foreach (var building in InitialBuildings)
            Create(building.pos, building.config);
    }
    /// <summary>
    /// Creates a building and saves it to the tile dictionary
    /// </summary>
    /// <param name="position"></param>
    /// <param name="buildingSO"></param>
    /// <returns></returns>
    public bool Create(Vector2Int position, BuildingScriptableObject buildingSO)
    {
        if (AreTilesOccupiedForBuilding(position, buildingSO))
            return false;
        var obj = buildingSO.CreateInstance(position);
        obj.transform.position = new Vector3(position.x, position.y, 0);
        for (int i = 0; i < buildingSO.size.y; i++)
            for (int j = 0; j < buildingSO.size.x; j++)
                tileDictionary[position + new Vector2Int(j, i)] = obj;

        return true;
    }
    /// <summary>
    /// Checks if the specific tile is occupied
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsTileOccupied(Vector2Int position)
    {
        return tileDictionary.ContainsKey(position);
    }
    /// <summary>
    /// Checks if any of the tiles from the list are occupied
    /// </summary>
    /// <param name="listOfPositions"></param>
    /// <returns></returns>
    public bool IsTileOccupied(List<Vector2Int> listOfPositions)
    {
        foreach (Vector2Int position in listOfPositions)
        {
            if (tileDictionary.ContainsKey(position)) return true;
        }
        return false;
    }
    /// <summary>
    /// Checks if all of the tiles the building would take are free.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="buildingSO"></param>
    /// <returns></returns>
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

        for (int i = -1; i <= top; i++)
        {
            for (int j = -1; j <= right; j++)
            {
                // for the sake of readability
                bool is_topleft_corner = (i == top && j == -1);
                bool is_topright_corner = (i == top && j == right);
                bool is_bottomleft_corner = (i == -1 && j == -1);
                bool is_bottomright_corner = (i == -1 && j == right);

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
    /// This method was made to allow pipes to appear on a tilemap.
    /// This was done so that there would not be excess pipe 
    /// objects in scene, and the FPS cost would be less.
    /// </summary>
    /// <param name="pipeSprite"></param>
    /// <returns></returns>
    public void AddTileToXY(Vector3Int position, Sprite pipeSprite) {
        PlaceTile(position, sprite);
    }

    // Method to place a tile with a given sprite at a given position
    private void PlaceTile(Vector3Int position, Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("Sprite is null. Cannot place tile.");
            return;
        }

        // Create a new Tile and assign the sprite to it
        Tile newTile = ScriptableObject.CreateInstance<Tile>();
        newTile.sprite = sprite;

        // Set the tile on the tilemap
        tilemap.SetTile(position, newTile);
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
                if (x >= building.Anchor.x && x <= upperRight.x-1 && y >= building.Anchor.y && y <= upperRight.y-1) continue;
                //if (new Vector2Int(xDistance, yDistance).sqrMagnitude <= range * range)
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
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (var building in InitialBuildings)
        {
            Vector3 middle = new Vector3(building.pos.x, building.pos.y, 0);
            middle += new Vector3(building.config.size.x, building.config.size.y, 0)/2;
            Vector3 size = new Vector3(building.config.size.x, building.config.size.y, 0);
            Gizmos.DrawCube(middle, size);
        }
    }
#endif
}