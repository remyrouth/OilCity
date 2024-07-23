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
    public override void OnTick()
    {
        if (_sequenceActions.Count == 0)
            GenerateNewSequence();
        _sequenceActions.Dequeue().Invoke(this);
    }
    private void GenerateNewSequence()
    {   
        //setup for incoming searching
        _sequenceActions.Enqueue((e) => { e._tilesSearched.Clear(); });
        
        // go to each spot
        for (int i = 0; i < GetNumberOfSearchingPoints(); i++)
            _sequenceActions.Enqueue((e) => { e.SearchForOil(); });
        
        //get worker back to building
        _sequenceActions.Enqueue((e) => { e.ResetWorker(); });
        
        //wait for the cooldown
        for (int i = 0; i < TickNumberInterval; i++)
            _sequenceActions.Enqueue((e) => { });
        
        //indicate best spot
        _sequenceActions.Enqueue((e) => { e.FinalizeSearching(); });
    }
    private void ResetWorker()
    {
        _workerVisual.DOKill();
        Vector3 pos = new Vector3(anchor.x, anchor.y, 0);
        _workerVisual.DOMove(pos, TimeManager.Instance.TicksPerMinute);
    }
    private void SearchForOil()
    {
        var tile = GetRandomWithinRange();
        if (tile == null)
            return;
        Vector3 pos = new Vector3(tile!.Value.x, tile!.Value.y);
        _workerVisual.DOKill();
        _workerVisual.DOMove(pos, TimeManager.Instance.TimePerTick / 2);
        _tilesSearched.Add(tile!.Value);
    }
    private void FinalizeSearching()
    {
        var bestOilSpot = _tilesSearched
            .OrderBy(e => BoardManager.Instance.OilEvaluator.GetValueAtPosition(e.x, e.y))
            .FirstOrDefault();

        if (bestOilSpot == null)
            return;

        Debug.Log($"Found great oil spot at {bestOilSpot}!");
        //indicate that position

    }
    private Vector2Int? GetRandomWithinRange()
    {
        var tiles = GetTilesInRange().Where(e => !BoardManager.Instance.IsTileOccupied(e));
        if (!tiles.Any())
            return null;
        return tiles.ToList()[UnityEngine.Random.Range(0, tiles.Count())];
    }
}
