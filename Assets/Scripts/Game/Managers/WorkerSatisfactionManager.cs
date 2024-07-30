using System;
using System.Linq;
using UnityEngine;

public class WorkerSatisfactionManager : Singleton<WorkerSatisfactionManager>, ITickReceiver
{
    public int WorkerSatisfaction { get; private set; }
    [SerializeField] private int InitialSatisfaction = 100;
    private int _tickTimer;
    private int PaymentTimer => 5;
    public event Action<int> OnWorkersSatisfactionChanged;
    private void Start()
    {
        TimeManager.Instance.RegisterReceiver(gameObject);
        WorkerSatisfaction = InitialSatisfaction;
        OnWorkersSatisfactionChanged?.Invoke(WorkerSatisfaction);
    }
    public void IncreaseSatisfaction(int amount) => ChangeSatisfaction(amount);
    public void DecreaseSatisfaction(int amount)
    {
        ChangeSatisfaction(-amount);
        if (amount == 0)
            GameOver();
    }
    private void ChangeSatisfaction(float delta)
    {
        WorkerSatisfaction = (int)Mathf.Clamp(WorkerSatisfaction + delta, 0, 100);
        OnWorkersSatisfactionChanged?.Invoke(WorkerSatisfaction);
    }

    public void GameOver()
    {
        UIStateMachine.Instance.ChangeState(GameState.EndingUI);
    }

    public void OnTick()
    {
        _tickTimer++;
        if(_tickTimer == PaymentTimer)
        {
            _tickTimer = 0;
            ChangeSatisfaction(BoardManager.Instance.tileDictionary.Values.OfType<PayrateBuildingController>().Select(e => e.GetIndexOfSatisfaction()).Sum());
        }
    }
}
