using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainStationController : BuildingController<BuildingScriptableObject>, IFlowable
{
    private List<IFlowable> _children = new();
    private const int TRAIN_INTERVAL = 10;

    [SerializeField] private Transform train;
    private Animator _trainAnimator;
    [SerializeField] private Vector3 startPos, arrivedPos, endPos;
    [SerializeField] private SingleSoundPlayer trainSequenceSFX, OilSellSFX;

    private void Awake()
    {
        _trainAnimator = train.GetComponent<Animator>();
    }
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

        //train arrives to station and waits
        // _sequenceActions.Enqueue(e => GetComponent<SingleSoundPlayer>().ActivateWithForeignTrigger());
        _sequenceActions.Enqueue(e => e._trainAnimator.SetTrigger("Arrive"));
        _sequenceActions.Enqueue(null);
        _sequenceActions.Enqueue(null);
        _sequenceActions.Enqueue(null);

        _sequenceActions.Enqueue((e) => { e._trainAnimator.SetTrigger("Load"); e.SellKerosene(); });

        //train leaves
        _sequenceActions.Enqueue((e) => e._trainAnimator.SetTrigger("Depart"));
        _sequenceActions.Enqueue(null);
    }

    private bool activationCheck = false;   // 

    private void CompleteTrainSystem()
    {
        if (!activationCheck)
        {
            Invoke("StartTrain", 0f);
            activationCheck = true;
            _sequenceActions.Enqueue(e => { });
        }

    }

    private void StartTrain()
    {
        train.localPosition = startPos;
        train.DOKill();
        trainSequenceSFX.ActivateWithForeignTrigger();
        // Debug.Log("Started");
        Invoke("EnterTrain", 8f);
    }

    private void EnterTrain()
    {
        train.DOKill();
        train.DOLocalMove(arrivedPos, 7f);
        // StartCoroutine(LerpToDestination(startPos, arrivedPos, 3f));
        Invoke("StayTrain", 7f);
    }

    private void StayTrain()
    {
        train.DOKill();
        // Debug.Log("StayTrain");
        Invoke("ExitTrain", 6f);
    }

    private void ExitTrain()
    {
        SellKerosene();
        OilSellSFX.ActivateWithForeignTrigger();
        train.DOLocalMove(endPos, 5);
        // StartCoroutine(LerpToDestination(arrivedPos, endPos, 5f));
        // train.position = Vector3.Lerp(arrivedPos, endPos, 5f);
        // LerpToDestination(arrivedPos, endPos, 5);
        // Debug.Log("ExitTrain");
        Invoke("EndTrain", 5f);
    }

    private void EndTrain()
    {
        _sequenceActions.Enqueue(null);
        activationCheck = false;
    }

    private void SellKerosene()
    {
        PopupValuesPool.Instance.GetFromPool<SimpleTextPopup>(PopupValuesPool.PopupValueType.EarnedMoney)
            .Initialize(((int)KeroseneManager.Instance.AmountToSell() * KeroseneManager.Instance.GetKerosenePrice()).ToString(), ActionsPivot);
        KeroseneManager.Instance.SellKerosene();
    }

    public (FlowType in_type, FlowType out_type) GetFlowConfig()
    {
        return (FlowType.Kerosene, FlowType.None);
    }


}
