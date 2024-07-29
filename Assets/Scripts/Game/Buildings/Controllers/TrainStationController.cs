using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainStationController : BuildingController<BuildingScriptableObject>, IFlowable
{
    private List<IFlowable> _children = new();
    private const int TRAIN_INTERVAL = 10;

    [SerializeField] private Transform train;
    [SerializeField] private Vector3 startPos, arrivedPos, endPos;

    public void AddChild(IFlowable child)
    {
        if (!_children.Contains(child))
            _children.Add(child);
    }

    public void DisownChild(IFlowable child)
    {
        if (_children.Contains(child))
            _children.Remove(child);
    }

    public List<IFlowable> GetChildren() => _children;

    public (bool can_input, bool can_output) GetFlowConfig() => (true, false);

    public IFlowable GetParent() => null;

    private readonly Queue<Action<TrainStationController>> _sequenceActions = new();
    public void OnTick()
    {
        if (_sequenceActions.Count == 0)
            GenerateNewSequence();
        _sequenceActions.Dequeue()?.Invoke(this);

        foreach (var child in _children)
        {
            var fluid = child.SendFlow();
            if (fluid.type == FlowType.Oil)
            {
                Debug.LogWarning("Train station got oil!!!");
                continue;
            }
            KeroseneManager.Instance.IncreaseAmount(fluid.amount);
        }
    }

    public (FlowType type, float amount) SendFlow() => (FlowType.None, 0);

    public void SetParent(IFlowable parent) { }
    private void GenerateNewSequence()
    {
        // wait for interval
        for (int i = 0; i < TRAIN_INTERVAL; i++)
            _sequenceActions.Enqueue(null);

        //setup train
        _sequenceActions.Enqueue(e => { e.DOKill(); e.train.localPosition = e.startPos; });

        //train arrives to station and waits
        _sequenceActions.Enqueue(e => e.train.DOLocalMove(e.arrivedPos, 2));
        _sequenceActions.Enqueue(null);

        _sequenceActions.Enqueue((e) => { e.DOKill(); e.SellKerosene(); });

        //train leaves
        _sequenceActions.Enqueue((e) => { e.train.DOLocalMove(e.endPos, 2); });
        _sequenceActions.Enqueue(null);
    }
    private void SellKerosene() { KeroseneManager.Instance.SellKerosene(); }
}
