using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerSatisfactionManager : Singleton<WorkerSatisfactionManager>
{
    public int workerSatisfaction;
    public int maxWorkerSatisfaction => 100;
    public int minWorkerSatisfaction => 0;
    public event Action<int> OnWorkersSatisfactionChanged;
    public void IncreaseSatisfaction(int amount)
    {
        if (workerSatisfaction + amount > maxWorkerSatisfaction)
            workerSatisfaction = maxWorkerSatisfaction;
        else
            workerSatisfaction += amount;
        OnWorkersSatisfactionChanged?.Invoke(workerSatisfaction);
    }

    public void DecreaseSatisfaction(int amount)
    {
        if (workerSatisfaction - amount < minWorkerSatisfaction)
            workerSatisfaction = minWorkerSatisfaction;
        else
            workerSatisfaction -= amount;
        if (workerSatisfaction <= minWorkerSatisfaction)
            GameOver();
        OnWorkersSatisfactionChanged?.Invoke(workerSatisfaction);
    }

    public void GameOver()
    {
        Debug.Log("You lost...");
    }
}
