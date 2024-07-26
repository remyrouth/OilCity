using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public sealed class WoodCutterController : AOEBuildingController
{
    public override int TickNumberInterval => 2;

    public override int Range => 4;

    private bool _active = true;
    [SerializeField] private Transform _workerVisual;

    private Queue<Action<WoodCutterController>> _sequenceActions = new();
    private Vector2Int? _focusedTree = null;

    public event Action<Vector2Int> OnTreeCutted;
    public override void OnTick()
    {
        if (!_active)
        {
            TimeManager.Instance.DeregisterReceiver(gameObject);
            return;
        }
        if (_sequenceActions.Count == 0)
            GenerateNewSequence();
        _sequenceActions.Dequeue()?.Invoke(this);
    }
    private void GenerateNewSequence()
    {
        //setup for incoming searching
        _sequenceActions.Enqueue((e) => { e._focusedTree = null; });

        // go to spot
        _sequenceActions.Enqueue((e) => { e.PickTree(); });
        _sequenceActions.Enqueue(null);
        _sequenceActions.Enqueue((e) => { e.FinalizeCutting(); });

        //wait for the cooldown
        for (int i = 0; i < TickNumberInterval; i++)
            _sequenceActions.Enqueue(null);
    }
    private void ResetWorker()
    {
        _workerVisual.DOKill();
        Vector3 pos = new Vector3(Anchor.x + 0.5f, Anchor.y, 0);
        _workerVisual.DOMove(pos, TimeManager.Instance.TimePerTick * 2);
    }
    private void PickTree()
    {
        _focusedTree = GetRandomWithinRange();
        if (_focusedTree == null)
            return;
        Vector3 pos = new Vector3(_focusedTree!.Value.x, _focusedTree!.Value.y) + new Vector3(0.5f, 0.5f, 0);
        _workerVisual.DOKill();
        _workerVisual.DOMove(pos, TimeManager.Instance.TimePerTick * 2);
    }
    private void FinalizeCutting()
    {
        if (_focusedTree == null)
        {
            ResetWorker();
            return;
        }
        BoardManager.Instance.Destroy(BoardManager.Instance.tileDictionary[_focusedTree!.Value]);
        OnTreeCutted?.Invoke(_focusedTree!.Value);
    }
    private Vector2Int? GetRandomWithinRange()
    {
        var tiles = GetTilesInRange()
            .Where(e => BoardManager.Instance.IsTileOccupied(e))
            .Where(e => BoardManager.Instance.tileDictionary[e] is TreeController)
            .ToList();
        if (!tiles.Any())
        {
            _active = false;
            return null;
        }
        Vector2 middle = new Vector2(Anchor.x + config.size.x / 2, Anchor.y + config.size.y / 2);
        tiles = tiles.OrderBy(e => (e - middle).sqrMagnitude).Take(Mathf.CeilToInt((float)tiles.Count() / 4)).ToList();
        var tile = tiles[UnityEngine.Random.Range(0, tiles.Count)];
        return tiles.ToList()[UnityEngine.Random.Range(0, tiles.Count())];
    }


}
