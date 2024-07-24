using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System;

public sealed class GeologistController : AOEBuildingController
{
    [SerializeField]
    private Transform _workerVisual;
    public override int TickNumberInterval => 10;

    public override int Range => 4;

    private int GetNumberOfSearchingPoints() => 3;

    private HashSet<Vector2Int> _tilesSearched = new();
    private Queue<Action<GeologistController>> _sequenceActions = new();

    public event Action<Vector2Int> OnOilSpotFound;

    public override void OnTick()
    {
        if (_sequenceActions.Count == 0)
            GenerateNewSequence();
        _sequenceActions.Dequeue()?.Invoke(this);
    }
    private void GenerateNewSequence()
    {
        //setup for incoming searching
        _sequenceActions.Enqueue((e) => { e._tilesSearched.Clear(); });

        int numOfSearchingPoint = GetNumberOfSearchingPoints();
        // go to each spot
        for (int i = 0; i < numOfSearchingPoint; i++)
        {
            _sequenceActions.Enqueue((e) => { e.SearchForOil(); });
            _sequenceActions.Enqueue(null);
            _sequenceActions.Enqueue(null);
        }

        //get worker back to building
        _sequenceActions.Enqueue((e) => { e.ResetWorker(); });

        //wait for the cooldown
        for (int i = 0; i < TickNumberInterval; i++)
            _sequenceActions.Enqueue(null);

        //indicate best spot
        _sequenceActions.Enqueue((e) => { e.FinalizeSearching(); });
    }
    private void ResetWorker()
    {
        _workerVisual.DOKill();
        Vector3 pos = new Vector3(Anchor.x, Anchor.y, 0);
        _workerVisual.DOMove(pos, TimeManager.Instance.TimePerTick);
    }
    private void SearchForOil()
    {
        var tile = GetRandomWithinRange();
        if (tile == null)
            return;
        Vector3 pos = new Vector3(tile!.Value.x, tile!.Value.y) + new Vector3(0.5f, 0.5f, 0);
        _workerVisual.DOKill();
        _workerVisual.DOMove(pos, TimeManager.Instance.TimePerTick * 2);
        _tilesSearched.Add(tile!.Value);
    }
    private void FinalizeSearching()
    {
        var bestOilSpot = _tilesSearched
            .Where(e => !BoardManager.Instance.IsTileOccupied(e)) //make sure that tile is still empty
            .OrderBy(e => BoardManager.Instance.OilEvaluator.GetValueAtPosition(e.x, e.y))
            .FirstOrDefault();

        if (bestOilSpot == null)
            return;

        Debug.Log($"Found great oil spot at {bestOilSpot}!");
        OnOilSpotFound?.Invoke(bestOilSpot);

    }
    private Vector2Int? GetRandomWithinRange()
    {
        var tiles = GetTilesInRange().Where(e => !BoardManager.Instance.IsTileOccupied(e));
        if (!tiles.Any())
            return null;
        return tiles.ToList()[UnityEngine.Random.Range(0, tiles.Count())];
    }
}
