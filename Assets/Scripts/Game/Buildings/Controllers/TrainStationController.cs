using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainStationController : BuildingController<BuildingScriptableObject>, IFlowable
{
    private List<IFlowable> _children = new();
    private const int TRAIN_INTERVAL = 80;

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

    public (bool can_input, bool can_output) GetInOutConfig() => (true, false);

    public IFlowable GetParent() => null;

    private readonly Queue<Action<TrainStationController>> _sequenceActions = new();
    public void OnTick()
    {
        // if (_sequenceActions.Count == 0)
        //     GenerateNewSequence();
        if (!activationCheck) {
            // Debug.Log("OnTick check happened");
            CompleteTrainSystem();
            // GenerateNewSequence();
        }



        // _sequenceActions.Dequeue()?.Invoke(this);

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
    // private void GenerateNewSequence()
    // {
    //     // wait for interval
    //     for (int i = 0; i < TRAIN_INTERVAL; i++)
    //         _sequenceActions.Enqueue(null);

    //     //setup train
    //     _sequenceActions.Enqueue(e => { e.DOKill(); e.train.localPosition = e.startPos; });

    //     //train arrives to station and waits
    //     // _sequenceActions.Enqueue(e => GetComponent<SingleSoundPlayer>().ActivateWithForeignTrigger());
    //     _sequenceActions.Enqueue(e => e.train.DOLocalMove(e.arrivedPos, 2));
    //     _sequenceActions.Enqueue(null);

    //     _sequenceActions.Enqueue((e) => { e.DOKill(); e.SellKerosene(); });

    //     //train leaves
    //     _sequenceActions.Enqueue((e) => { e.train.DOLocalMove(e.endPos, 2); });
    //     _sequenceActions.Enqueue(null);
    // }

    private bool activationCheck = false;

    private void CompleteTrainSystem()
    {
        if (!activationCheck) {
            Invoke("StartTrain", 0f);
            activationCheck = true;
            _sequenceActions.Enqueue(e => {});
        }

    }


    private void StartTrain() {
        train.localPosition = startPos;
        train.DOKill();
        GetComponent<SingleSoundPlayer>().ActivateWithForeignTrigger();
        // Debug.Log("Started");
        Invoke("EnterTrain", 8f);
    }

    private void EnterTrain() {
        train.DOKill();
        train.DOLocalMove(arrivedPos, 7f);
        // Debug.Log("EnterTrain");
        Invoke("StayTrain", 7f);
    }

    private void StayTrain() {
        train.DOKill();
        SellKerosene();
        // Debug.Log("StayTrain");
        Invoke("ExitTrain", 6f);
    }

    private void ExitTrain() {
        SellKerosene();
        train.DOLocalMove(endPos, 5);
        // Debug.Log("ExitTrain");
        Invoke("EndTrain", 5f);
    }

    private void EndTrain() {
        _sequenceActions.Enqueue(null);
        activationCheck = false;
    }

    private void SellKerosene() { KeroseneManager.Instance.SellKerosene(); }

    public (FlowType in_type, FlowType out_type) GetFlowConfig()
    {
        return (FlowType.Kerosene, FlowType.None);
    }
}
