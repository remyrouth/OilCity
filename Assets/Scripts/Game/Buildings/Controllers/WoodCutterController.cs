using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Events;
using UnityEngine;
public sealed class WoodCutterController : AOEBuildingController
{
    class WoodCutterWorker
    {
        public Transform _workerVisual;
        public Queue<Action<WoodCutterController>> _sequenceActions = new();
        public Vector2Int? _focusedTree = null;
        public bool _isActive = true;
        public WoodCutterWorker(Transform worker)
        {
            _workerVisual = worker;
            _sequenceActions = new();
        }
    }
    public const float WORKERS_SPEED = 10;
    private int activeWorkerAmount;
    private List<WoodCutterWorker> _firedWorkers = new List<WoodCutterWorker>();
    public override int TickNumberInterval => 0;
    [SerializeField] private int _workerAmount;
    [SerializeField] private Transform _workerVisualPrefab;
    private List<WoodCutterWorker> _workers = new List<WoodCutterWorker>();
    //private Queue<Action<WoodCutterController>> _sequenceActions = new();
    //private Vector2Int? _focusedTree = null;
    //private List<Vector2Int> treeList = new();
    public event Action<Vector2Int> OnTreeCutted;
    private int _tickTimer;
    private int PaymentTimer => 5;
    public int WorkersCount => _workers.Count;
    public void Start()
    {
        OnPaymentModeIncreased += IncreaseProductivity;
        OnPaymentModeDecreased += DecreaseProductivity;
        for (int i = 0; i < _workerAmount; i++)
        {
            CreateWorker();
            for (int j = 0; j < i; j++)
                _workers[i]._sequenceActions.Enqueue(null);
        }

    }
    public override void OnTick()
    {
        for (int i = WorkersCount - 1; i >= 4; i--)
        {
            FireWorker(_workers[i]);
        }
        _tickTimer++;
        if (_tickTimer == PaymentTimer)
        {
            _tickTimer = 0;
            PayWorkers();
        }
        activeWorkerAmount = _workers.Where(e => e._isActive).Count();
        for (int i = _firedWorkers.Count - 1; i >= 0; i--)
        {
            _firedWorkers[i]._sequenceActions.Dequeue()?.Invoke(this);
            if (_firedWorkers[i]._sequenceActions.Count == 0)
                _firedWorkers.RemoveAt(i);
        }
        for (int i = _workers.Count - 1; i >= 0; i--)
        {
            if (_workers[i]._sequenceActions.Count == 0)
                GenerateNewSequence(_workers[i]);

            _workers[i]._sequenceActions.Dequeue()?.Invoke(this);
            if (i == _workers.Count - 1)
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
        worker._sequenceActions.Enqueue(null);
        worker._sequenceActions.Enqueue((e) => { e.FinalizeCutting(worker); });

        //wait for the cooldown
        for (int i = 0; i < TickNumberInterval; i++)
            worker._sequenceActions.Enqueue(null);
    }
    private void ResetWorker(Transform worker)
    {
        worker.DOKill();
        int index = _workers.Select(e => e._workerVisual).ToList().IndexOf(worker);
        Vector3 pos = new Vector3(Anchor.x + index * 0.5f, Anchor.y, 0);

        var anim = worker.GetComponent<WorkerVisualAnimator>().Anim;
        worker.GetComponent<SpriteRenderer>().flipX = pos.x < worker.position.x;
        anim.SetBool("IsWalking", true);

        worker.DOMove(pos, TimeManager.Instance.TimePerTick * 2)
            .OnComplete(() => { anim.SetBool("IsWalking", false); });
    }
    private void PickTree(WoodCutterWorker worker)
    {
        Vector2Int? tree;
        tree = GetRandomWithinRange();
        if (tree == null) return;
        worker._focusedTree = tree;
        Vector3 pos = new Vector3(worker._focusedTree!.Value.x, worker._focusedTree!.Value.y) + new Vector3(0.5f, 0.5f, 0);

        var anim = worker._workerVisual.GetComponent<WorkerVisualAnimator>().Anim;
        worker._workerVisual.GetComponent<SpriteRenderer>().flipX = pos.x < worker._workerVisual.position.x;
        anim.SetBool("IsWalking", true);

        worker._workerVisual.DOKill();
        worker._workerVisual.DOMove(pos, TimeManager.Instance.TimePerTick * 2.25f)
            .OnComplete(() => { anim.SetBool("IsWalking", false); anim.SetTrigger("DoAction"); });
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
    private void CreateWorker()
    {
        int index = _workers.Count;
        Vector3 pos = new Vector3(Anchor.x + index * 0.5f, Anchor.y, 0);
        var tmp = new WoodCutterWorker(Instantiate(_workerVisualPrefab, pos, Quaternion.identity));
        tmp._workerVisual.SetParent(transform);
        _workers.Add(tmp);
    }
    private void FireWorker(WoodCutterWorker worker)
    {
        _firedWorkers.Add(worker);
        _workers.Remove(worker);
        worker._isActive = false;
        activeWorkerAmount = _workers.Where(e => e._isActive).Count();
        worker._sequenceActions.Clear();
        worker._focusedTree = null;
        worker._sequenceActions.Enqueue((e) => e.ResetWorker(worker._workerVisual));
        worker._sequenceActions.Enqueue(null);
        worker._sequenceActions.Enqueue((e) => e.DestroyWorker(worker));
    }
    private void DestroyWorker(WoodCutterWorker worker)
    {
        Destroy(worker._workerVisual.gameObject);
    }
    protected override void IncreaseProductivity()
    {
        BuildingEvents.IncreasePayment();
        switch (CurrentPaymentMode)
        {
            case PaymentMode.MEDIUM:
                CreateWorker();
                break;
            case PaymentMode.HIGH:
                CreateWorker();
                _workers.Last()._sequenceActions.Enqueue(null);
                CreateWorker();
                break;
            default:
                break;
        }
    }
    protected override void DecreaseProductivity()
    {
        switch (CurrentPaymentMode)
        {
            case PaymentMode.MEDIUM:
                FireWorker(_workers[WorkersCount - 1]);
                activeWorkerAmount = _workers.Where(e => e._isActive).Count();
                FireWorker(_workers[WorkersCount - 1]);
                break;
            case PaymentMode.LOW:
                FireWorker(_workers[WorkersCount - 1]);
                break;
            default:
                break;
        }
    }
}
