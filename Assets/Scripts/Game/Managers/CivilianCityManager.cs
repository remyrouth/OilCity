using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CivilianCityManager : Singleton<CivilianCityManager>, ITickReceiver
{
    [field: SerializeField] private BuildingScriptableObject civilianBuildingSO;

    private int _tickTimer = 0;
    private int _decayTimer = 0;
    public const int BASE_BUILD_TICK_INTERVAL = 10;
    public const int STARTING_INITIAL_BUILDINGS = 3;

    public const int DECAY_LENGTH = 30;
    private readonly Queue<(Vector2Int, float)> _destroyedTTL = new();
    public int NumOfBuildings = 0;
    private void Awake()
    {
        BoardManager.Instance.OnBuildingDestroyed += HandleHouseDecay;
    }
    public void Start()
    {
        TimeManager.Instance.RegisterReceiver(this);
        for (int i = 0; i < STARTING_INITIAL_BUILDINGS; i++)
        {
            InvokeAction();
        }
    }
    /// <summary>
    /// Checks how much worker satisfaction the player currently has, based on that it changes the tickNumberInterval
    /// impacting the frequency of InvokeAction execution.
    /// </summary>
    /// 
    public void OnTick()
    {
        _tickTimer++;
        _decayTimer++;
        if (_tickTimer >= GetTickNumberInterval())
        {
            _tickTimer = 0;
            InvokeAction();
        }

        while (_destroyedTTL.Count > 0 && _destroyedTTL.Peek().Item2 <= _decayTimer)
            _destroyedTTL.Dequeue();

    }
    private void HandleHouseDecay(Vector2Int pos, TileObjectController toc)
    {
        if (toc as CivilianBuildingController == null)
            return;
        _destroyedTTL.Enqueue((pos, _decayTimer + DECAY_LENGTH));
    }
    private int GetTickNumberInterval()
    {
        var cws = WorkerSatisfactionManager.Instance.WorkerSatisfaction;
        if (cws >= 90)
            return (int)(BASE_BUILD_TICK_INTERVAL * 0.3f);
        if (cws >= 70)
            return (int)(BASE_BUILD_TICK_INTERVAL * 0.5f);
        if (cws >= 50)
            return (int)(BASE_BUILD_TICK_INTERVAL * 0.8f);
        return BASE_BUILD_TICK_INTERVAL;
    }
    /// <summary>
    /// Tries to build a random civilian building around a player placed building.
    /// Looks for a building placed by the player to build around, in case no building exists, it stops. After finding a building, checks if any of the nearby tiles are free.
    /// If doesn't find a free tile, the range in which it tries to place a building increases.
    /// </summary>
    public void InvokeAction()
    {
        var buildings = FindObjectsByType<TileObjectController>(FindObjectsSortMode.None)
            .Where(e => e is CivilianBuildingController).ToList();
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
            List<Vector2Int> freeTiles = BoardManager.Instance
                .GetTilesInRange(civilianBuilding, i)
                .Where(e => !BoardManager.Instance.IsTileOccupied(e) && !_destroyedTTL.Any(x => x.Item1 == e))
                .ToList();
            if (freeTiles.Any())
            {
                Vector2Int pos = freeTiles[Random.Range(0, freeTiles.Count)];
                BoardManager.Instance.Create(pos, civilianBuildingSO);
                NumOfBuildings++;
                break;
            }
        }

    }
}
