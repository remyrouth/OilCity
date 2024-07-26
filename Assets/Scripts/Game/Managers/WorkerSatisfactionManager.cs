using System;
using UnityEngine;

public class WorkerSatisfactionManager : Singleton<WorkerSatisfactionManager>
{
    public int WorkerSatisfaction { get; private set; }
    public event Action<int> OnWorkersSatisfactionChanged;
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
        Debug.Log("You lost...");
    }
}
