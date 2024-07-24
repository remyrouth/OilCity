using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerSatisfactionManager : Singleton<WorkerSatisfactionManager>
{
    public int workerSatisfaction { get; private set; }
    public int maxWorkerSatisfaction => 100;
    public int minWorkerSatisfaction => 0;

    public void increaseSatisfaction(int amount)
    {
        if (workerSatisfaction + amount > maxWorkerSatisfaction)
            workerSatisfaction = maxWorkerSatisfaction;
        else
            workerSatisfaction += amount;
    }

    public void decreaseSatisfaction(int amount)
    {
        if (workerSatisfaction - amount > minWorkerSatisfaction)
            workerSatisfaction = minWorkerSatisfaction;
        else
            workerSatisfaction -= amount;
        if (workerSatisfaction <= minWorkerSatisfaction)
            gameOver();
    }

    public void gameOver()
    {
        Debug.Log("You lost...");
    }
}
