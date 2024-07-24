using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CivilianCityManager : Singleton<CivilianCityManager>, ITickReceiver
{
    [field: SerializeField] private BuildingScriptableObject civilianBuilding;
    private int _tickTimer = 0;
    public int tickNumberInterval = 10;
    
    /// <summary>
    /// Checks how much worker satisfaction the player currently has, based on that it changes the tickNumberInterval
    /// impacting the frequency of InvokeAction execution.
    /// </summary>
    public void OnTick()
    {
        int currWorkerSatisfaction = WorkerSatisfactionManager.Instance.workerSatisfaction;
        if (currWorkerSatisfaction >= 90)
            tickNumberInterval = 3;
        else if (currWorkerSatisfaction >= 70)
            tickNumberInterval = 5;
        else if (currWorkerSatisfaction >= 50)
            tickNumberInterval = 8;
        else
            tickNumberInterval = 10;
        _tickTimer++;
        if (_tickTimer > tickNumberInterval)
        {
            _tickTimer = 0;
            InvokeAction();
        }
    }
    /// <summary>
    /// Tries to build a civilian building around a player placed building.
    /// Looks for a building placed by the player to build around, in case no building exists, it stops. After finding a building, checks if any of the nearby tiles are free.
    /// If doesn't find a free tile, the range in which it tries to place a building increases.
    /// </summary>
    public void InvokeAction()
    {
        TileObjectController building = null;
        TileObjectController[] tmpArray = FindObjectsByType<TileObjectController>(FindObjectsSortMode.None);
        List<TileObjectController> tmpList = tmpArray.ToList();
        if (tmpList.Count <= 0)
            return;
        while (building is null)
        {
            if (tmpList.Count <= 0)
                return;
            int range = Random.Range(0, tmpList.Count);
            if (tmpList[range].TryGetComponent<TreeController>(out _))
            {
                tmpList.RemoveAt(range);
                continue;
            }
            building = tmpList[range];
        }
        List<Vector2Int> freeTiles = GetTilesInRange(building, 2).Where(e => !BoardManager.Instance.IsTileOccupied(e)).ToList();
        for (int i = 3; i >= 10; i++)
        {
            if (freeTiles.Any())
            {
                BoardManager.Instance.Create(freeTiles[Random.Range(0, freeTiles.Count)], civilianBuilding);
                break;
            }
            freeTiles = GetTilesInRange(building, i).Where(e => !BoardManager.Instance.IsTileOccupied(e)).ToList();
        }

    }
    //Same code as in the AOEBuilding Controller
    private List<Vector2Int> GetTilesInRange(TileObjectController building, int range)
    {
    Vector2Int upperRight = building.Anchor + building.size;
        List<Vector2Int> tiles = new();



        for (int x = building.Anchor.x - range; x <= upperRight.x + range; x++)
        {
            for (int y = building.Anchor.y - range; y <= upperRight.y + range; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                int xDistance = Mathf.Max(0, building.Anchor.x - currentPos.x, currentPos.x - upperRight.x);
                int yDistance = Mathf.Max(0, building.Anchor.y - currentPos.y, currentPos.y - upperRight.y);

                if (new Vector2Int(xDistance, yDistance).sqrMagnitude <= range * range)
                    tiles.Add(currentPos);
            }
        }

        return tiles;
    }
}