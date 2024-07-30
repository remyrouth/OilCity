using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CivilianCityManager : Singleton<CivilianCityManager>, ITickReceiver
{
    [field: SerializeField] private BuildingScriptableObject civilianBuildingSO;
    
    private int _tickTimer = 0;
    public int tickNumberInterval = 10;

    public void Start()
    {
        TimeManager.Instance.RegisterReceiver(this);
    }
    /// <summary>
    /// Checks how much worker satisfaction the player currently has, based on that it changes the tickNumberInterval
    /// impacting the frequency of InvokeAction execution.
    /// </summary>
    /// 
    public void OnTick()
    {
        int currWorkerSatisfaction = WorkerSatisfactionManager.Instance.WorkerSatisfaction;
        if (currWorkerSatisfaction >= 90)
            tickNumberInterval = 3;
        else if (currWorkerSatisfaction >= 70)
            tickNumberInterval = 5;
        else if (currWorkerSatisfaction >= 50)
            tickNumberInterval = 8;
        else
            tickNumberInterval = 10;
        _tickTimer++;
        if (_tickTimer >= tickNumberInterval)
        {
            _tickTimer = 0;
            InvokeAction();
        }
    }
    /// <summary>
    /// Tries to build a random civilian building around a player placed building.
    /// Looks for a building placed by the player to build around, in case no building exists, it stops. After finding a building, checks if any of the nearby tiles are free.
    /// If doesn't find a free tile, the range in which it tries to place a building increases.
    /// </summary>
    public void InvokeAction()
    {
        var buildings = FindObjectsByType<TileObjectController>(FindObjectsSortMode.None)
            .Where(e=>e is CivilianBuildingController).ToList();
        var centroidBuildings = FindObjectsByType<TileObjectController>(FindObjectsSortMode.None)
            .Where(e => e is not TreeController && e is not CivilianBuildingController).ToList();

        if (buildings.Count == 0)
            return;

        Vector2 centroid = Vector2.zero;
        foreach (var building in centroidBuildings)
            centroid += building.Anchor;
        centroid /= centroidBuildings.Count;

        var closest = buildings.OrderBy(e => (e.Anchor - centroid).sqrMagnitude).Take(10).ToList();

        TileObjectController civilianBuilding = closest[Random.Range(0, closest.Count)];

        for (int i = 2; i <= 10; i++)
        {
             List<Vector2Int> freeTiles = BoardManager.Instance.GetTilesInRange(civilianBuilding, i).Where(e => !BoardManager.Instance.IsTileOccupied(e)).ToList();
            if (freeTiles.Any())
            {
                Vector2Int pos = freeTiles[Random.Range(0,freeTiles.Count)];
                BoardManager.Instance.Create(pos, civilianBuildingSO);     
                break;
            }
        }

    }
}
