using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System;

public sealed class GeologistController : AOEBuildingController
{
    class GeologistWorker
    {
        public Transform _workerVisual;
        public Queue<Action<GeologistController>> _sequenceActions = new();
        public bool _isActive = false;
        public GeologistWorker(Transform worker)
        {
            _workerVisual = worker;
            _sequenceActions = new();

        }
    }
    List<GeologistWorker> _workers = new List<GeologistWorker>();
    List<GeologistWorker> _firedWorkers = new List<GeologistWorker>();
    [SerializeField] private Transform _workerVisual;
    [SerializeField] private GameObject _oilPingPrefab;
    [SerializeField] private int _workerAmount;
    public int activeWorkerAmount { get;private set; }
    public override int TickNumberInterval => 10;

    public override int Range => 4;

    private int GetNumberOfSearchingPoints()
    {
        return CurrentPaymentMode switch
        {
            PaymentMode.LOW => 2,
            PaymentMode.MEDIUM => 3,
            PaymentMode.HIGH => 4,
            _ => 0,
        };
    }

    private HashSet<Vector2Int> _tilesSearched = new();

   // public event Action<Vector2Int> OnOilSpotFound;

    public event Action<Vector2Int, float> OnOilSpotFound;

    private void Awake() => OnOilSpotFound += PingSpot;

    public void Start()
    {
        OnPaymentModeIncreased += IncreaseProductivity;
        OnPaymentModeDecreased += DecreaseProductivity;
        for (int i = 0; i < _workerAmount; i++)
        {
            CreateWorker();
        }
    }
    public override void OnTick()
    {
        for (int i = _firedWorkers.Count - 1; i >= 0; i--)
        {
            _firedWorkers[i]._sequenceActions.Dequeue()?.Invoke(this);
            if (_firedWorkers[i]._sequenceActions.Count == 0)
                _firedWorkers.RemoveAt(i);
        }
        for (int i = 0; i < _workers.Count; i++)
        {
            if (_workers[i]._sequenceActions.Count == 0)
            {
                PayWorkers();
                GenerateNewSequence(_workers[i]);
                _workers[i]._isActive = true;
                activeWorkerAmount = _workers.Where(e => e._isActive).Count(); ;
            }
            _workers[i]._sequenceActions.Dequeue()?.Invoke(this);
        }
    }
    private void GenerateNewSequence(GeologistWorker worker)
    {
        //setup for incoming searching
        worker._sequenceActions.Enqueue((e) => { e._tilesSearched.Clear(); });

        int numOfSearchingPoint = GetNumberOfSearchingPoints();
        // go to each spot
        for (int i = 0; i < numOfSearchingPoint; i++)
        {
            worker._sequenceActions.Enqueue((e) => { e.SearchForOil(worker); });
            worker._sequenceActions.Enqueue(null);
            worker._sequenceActions.Enqueue(null);
        }

        //get worker back to building
        worker._sequenceActions.Enqueue((e) => { e.ResetWorker(worker); });

        //indicate best spot
        if (!_workers.Any(e => e._isActive))
            FinalizeSearching();
        //wait for the cooldown
        /*for (int i = 0; i < TickNumberInterval-7; i++)*/
            worker._sequenceActions.Enqueue(null);
            worker._sequenceActions.Enqueue(null);
    }
    private void ResetWorker(GeologistWorker worker)
    {
        worker._isActive = false;
        worker._workerVisual.DOKill();
        Vector3 pos = new Vector3(Anchor.x, Anchor.y, 0);
        worker._workerVisual.DOMove(pos, TimeManager.Instance.TimePerTick);
    }
    private void SearchForOil(GeologistWorker worker)
    {
        var tile = GetRandomWithinRange();
        if (tile == null)
            return;
        Vector3 pos = new Vector3(tile!.Value.x, tile!.Value.y) + new Vector3(0.5f, 0.5f, 0);
        worker._workerVisual.DOKill();
        worker._workerVisual.DOMove(pos, TimeManager.Instance.TimePerTick * 2);
        _tilesSearched.Add(tile!.Value);
    }
    private void FinalizeSearching()
    {
        var bestOilSpot = _tilesSearched
            .Where(e => !BoardManager.Instance.IsTileOccupied(e)) // Ensure the tile is empty
            .OrderBy(e => BoardManager.Instance.OilEvaluator.GetValueAtPosition(e.x, e.y))
            .LastOrDefault();

        if (bestOilSpot == null)
            return;

        FireWorkersIfNeeded();
        Debug.Log($"Found great oil spot at {bestOilSpot}!");

        // Retrieve the oil value at the best spot
        float oilValue = BoardManager.Instance.OilEvaluator.GetValueAtPosition(bestOilSpot.x, bestOilSpot.y);

        oilValue = oilValue * 100;

        // Trigger the event with both the position and the oil value
        OnOilSpotFound?.Invoke(bestOilSpot, oilValue);
    }
    private void FireWorkersIfNeeded()
    {
        if(_workers.Count() > 3)
        {
            for (int i = _workers.Count() - 1; i >= 3; i--)
            {
                FireWorker(_workers[i]);
            }
        }
    }
    private void PingSpot(Vector2Int pos, float oilValue)
    {
        // Instantiate the ping prefab
        var obj = Instantiate(_oilPingPrefab, new Vector3(pos.x, pos.y, -0.01f), Quaternion.identity);

        // Set the scale animation
        obj.transform.localScale = Vector3.zero;
        obj.transform.DOScale(Vector3.one, 0.25f);

        // Find the text component in the prefab
        var textComponent = obj.GetComponentInChildren<TMPro.TextMeshPro>();

        if (textComponent != null)
        {
            // Set the text to display the oil value
            textComponent.text = oilValue.ToString() + "%";
        }

        // Destroy the ping object after 10 seconds
        Destroy(obj, 10);
    }
    private Vector2Int? GetRandomWithinRange()
    {
        var tiles = GetTilesInRange().Where(e => !BoardManager.Instance.IsTileOccupied(e));
        if (!tiles.Any())
            return null;
        return tiles.ToList()[UnityEngine.Random.Range(0, tiles.Count())];
    }

    private void CreateWorker()
    {
        var tmp = new GeologistWorker(Instantiate(_workerVisual, Anchor.ToVector3(), Quaternion.identity));
        tmp._workerVisual.SetParent(transform);
        if (_workers.Count() != 0)
        {
            for (int i = 0; i < _workers[0]._sequenceActions.Count; i++)
            {
                tmp._sequenceActions.Enqueue(null);
            }
        }
        _workers.Add(tmp);
    }
    private void FireWorker(GeologistWorker worker)
    {
        _firedWorkers.Add(worker);
        _workers.Remove(worker);
        worker._isActive = false;
        activeWorkerAmount = _workers.Where(e => e._isActive).Count();
        worker._sequenceActions.Clear();
        worker._sequenceActions.Enqueue((e) => e.ResetWorker(worker));
        worker._sequenceActions.Enqueue(null);
        worker._sequenceActions.Enqueue((e) => e.DestroyWorker(worker));
    }
    private void DestroyWorker(GeologistWorker worker)
    {
        Destroy(worker._workerVisual.gameObject);
    }

    protected override void IncreaseProductivity()
    {
        switch (CurrentPaymentMode)
        {
            case PaymentMode.MEDIUM:
                CreateWorker();
                break;
            case PaymentMode.HIGH:
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
                FireWorker(_workers[_workers.Count() - 1]);
                break;
            case PaymentMode.LOW:
                FireWorker(_workers[_workers.Count() - 1]);
                break;
            default:
                break;
        }
    }
}
