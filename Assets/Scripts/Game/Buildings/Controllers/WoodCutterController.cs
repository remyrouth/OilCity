using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public sealed class WoodCutterController : AOEBuildingController
{
    class WoodCutterWorker
    {
        public Transform _workerVisual;
        public Queue<Action<WoodCutterController>> _sequenceActions = new();
        public Vector2Int? _focusedTree = null;
        public bool _isActive = true;
        public WoodCutterWorker(Transform worker, Queue<Action<WoodCutterController>> sequenceActions)
        {
            _workerVisual = worker;
            _sequenceActions = sequenceActions;
            
        }
    }

    public override int TickNumberInterval => 0;
    public override int Range => 4;
    [SerializeField] private int _workerAmount;
    [SerializeField] private Transform _workerVisual;
    private List<WoodCutterWorker> _workers = new List<WoodCutterWorker>();
    //private Queue<Action<WoodCutterController>> _sequenceActions = new();
    //private Vector2Int? _focusedTree = null;
    //private List<Vector2Int> treeList = new();
    public event Action<Vector2Int> OnTreeCutted;
    private int _tickTimer;
    private int PaymentTimer => 5;
    public void Start()
    {
        for (int i = 0; i < _workerAmount; i++)
        {
            var tmp = new WoodCutterWorker(Instantiate(_workerVisual, Anchor.ToVector3(), Quaternion.identity), new());
            tmp._workerVisual.SetParent(transform);
            _workers.Add(tmp);
            for (int j = 0; j < i; j++)
                _workers[i]._sequenceActions.Enqueue(null);
        }
    }
    public override void OnTick()
    {
        _tickTimer++;
        if (_tickTimer == PaymentTimer)
        {
            _tickTimer = 0;
            PayWorkers();
        }
        if (!_workers.Any(e => e._isActive))
        {
            TimeManager.Instance.DeregisterReceiver(gameObject);
            return;
        }
        for (int i = 0; i < _workers.Count; i++)
        {
            if (_workers[i]._sequenceActions.Count == 0)
                GenerateNewSequence(_workers[i]);

            _workers[i]._sequenceActions.Dequeue()?.Invoke(this);
            if(i == _workers.Count-1)
                PayWorkers();
        }
    }
    private void GenerateNewSequence(WoodCutterWorker worker)
    {
        //setup for incoming searching
        //_sequenceActions.Enqueue((e) => { e._focusedTree = null; });

        // go to spot
        worker._sequenceActions.Enqueue((e) => { e.PickTree(worker); });
        worker._sequenceActions.Enqueue(null);
        worker._sequenceActions.Enqueue((e) => { e.FinalizeCutting(worker); });

        //wait for the cooldown
        for (int i = 0; i < TickNumberInterval; i++)
            worker._sequenceActions.Enqueue(null);
    }
    private void ResetWorker(Transform worker)
    {
        worker.DOKill();
        Vector3 pos = new Vector3(Anchor.x + 0.5f, Anchor.y, 0);
        worker.DOMove(pos, TimeManager.Instance.TimePerTick * 2);
    }
    private void PickTree(WoodCutterWorker worker)
    {
        Vector2Int? tree;
        tree = GetRandomWithinRange();
        if (tree == null) return;
        worker._focusedTree = tree;
        Vector3 pos = new Vector3(worker._focusedTree!.Value.x, worker._focusedTree!.Value.y) + new Vector3(0.5f, 0.5f, 0);
        worker._workerVisual.DOKill();
        worker._workerVisual.DOMove(pos, TimeManager.Instance.TimePerTick * 2);
    }
    private void FinalizeCutting(WoodCutterWorker worker)
    {
        if (worker._focusedTree == null)
        {
            ResetWorker(worker._workerVisual);
            worker._isActive = false;
            return;
        }
        BoardManager.Instance.Destroy(BoardManager.Instance.tileDictionary[worker._focusedTree!.Value]);
        worker._focusedTree = null;
        OnTreeCutted?.Invoke(worker._focusedTree!.Value);
    }
    private Vector2Int? GetRandomWithinRange()
    {
        var tiles = GetTilesInRange()
            .Where(e => BoardManager.Instance.IsTileOccupied(e))
            .Where(e => BoardManager.Instance.tileDictionary[e] is TreeController)
            .Where(e => !_workers.Any(w => w._focusedTree == e))
            .ToList();
        if (!tiles.Any())
            return null;

        Vector2 middle = new Vector2(Anchor.x + config.size.x / 2, Anchor.y + config.size.y / 2);
        tiles = tiles.OrderBy(e => (e - middle).sqrMagnitude).Take(Mathf.CeilToInt((float)tiles.Count() / 20)).ToList();
        var tile = tiles[UnityEngine.Random.Range(0, tiles.Count)];
        return tiles.ToList()[UnityEngine.Random.Range(0, tiles.Count())];
    }


}
