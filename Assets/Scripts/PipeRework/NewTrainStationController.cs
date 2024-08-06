using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NewTrainStationController : BuildingController<BuildingScriptableObject>, INewFlowable
{
    private const int TRAIN_INTERVAL = 10;

    [SerializeField] private Transform train;
    [SerializeField] private Vector3 startPos, arrivedPos, endPos;

    private readonly Queue<Action<NewTrainStationController>> _sequenceActions = new();

    private TreeRelationship m_tr = new(99, 0);

    public (bool can_input, bool can_output) GetInOutConfig() => (true, false);

    public void OnTick()
    {
        if (_sequenceActions.Count == 0)
            GenerateNewSequence();
        _sequenceActions.Dequeue()?.Invoke(this);

        foreach (var child in m_tr.GetChildren())
        {
            var fluid = child.SendFlow();

            /*
            if (fluid.type == FlowType.Oil)
            {
                Debug.LogWarning("Train station got oil!!!");
                continue;
            }
            */

            KeroseneManager.Instance.IncreaseAmount(fluid);
        }
    }

    public float SendFlow() => 0.0f;

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

    public (FlowType in_type, FlowType out_type) GetFlowConfig()
    {
        return (FlowType.Kerosene, FlowType.None);
    }

    public TreeRelationship GetTreeRelationship() => m_tr;

    public void UpdateConnections(ISet<INewFlowable> seen)
    {
        if (seen.Contains(this)) return;

        seen.Add(this);

        // bit dumb to continue the recursion, but it won't break anything
        // this is so because this method is invoked by relationships, of which a train
        // station only has one of. Therefore, the only place it can send the signal to
        // is the place it got it from.
        // 
        // but whatever. it keeps the code tidy to have a pattern, so i'll keep it.

        foreach (var child in m_tr.GetChildren()) child.UpdateConnections(seen);
        foreach (var parent in m_tr.GetParents()) parent.UpdateConnections(seen);
        foreach (var tentative in m_tr.GetTentative(Relation.Ambiguous)) tentative.flowable.UpdateConnections(seen);
    }
}
