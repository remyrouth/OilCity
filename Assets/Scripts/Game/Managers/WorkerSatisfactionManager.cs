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

    [SerializeField] private SingleSoundPlayer workerIncreaseSfX;
    [SerializeField] private SingleSoundPlayer workerDecreaseSfX;
    private void Start()
    {
        TimeManager.Instance.RegisterReceiver(this);
        WorkerSatisfaction = InitialSatisfaction;
        OnWorkersSatisfactionChanged += CheckForGameOver;
        OnWorkersSatisfactionChanged?.Invoke(WorkerSatisfaction);
    }
    public void IncreaseSatisfaction(int amount) => ChangeSatisfaction(amount);
    public void DecreaseSatisfaction(int amount)
    {
        ChangeSatisfaction(-amount);

    }
    private void ChangeSatisfaction(float delta)
    {
        WorkerSatisfaction = (int)Mathf.Clamp(WorkerSatisfaction + delta, 0, 100);
        OnWorkersSatisfactionChanged?.Invoke(WorkerSatisfaction);


        if (delta > 0) {
            // we trigger positive increase sfx
            Debug.Log("Increase sfx sound here");
            workerIncreaseSfX.ActivateWithForeignTrigger();
        } else if (delta == 0f) {
            // we skip
        } else {
            // we trigger negative decrease sfx
            workerDecreaseSfX.ActivateWithForeignTrigger();
        }
    }
    private void CheckForGameOver(int newValue)
    {
        if (newValue <= 0)
            GameStateManager.Instance.EndGame();
    }

    public void OnTick()
    {
        _tickTimer++;
        if (_tickTimer == PaymentTimer)
        {
            _tickTimer = 0;
            ChangeSatisfaction(BoardManager.Instance.tileDictionary.Values.OfType<PayrateBuildingController>().Select(e => e.GetIndexOfSatisfaction()).Sum() * .25f);
        }
    }
}
