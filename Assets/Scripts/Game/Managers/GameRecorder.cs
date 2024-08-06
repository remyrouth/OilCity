using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRecorder : Singleton<GameRecorder>
{
    private readonly Queue<(float, Action)> actions = new();

    public void CreateSnapshot(Action revertAction)
    {
        float percentage = TimeLineEventManager.Instance.GetTimePercentage();
        actions.Enqueue((percentage, revertAction));
    }
    private const float ROLLBACK_SPEED = 0.05f;
    public IEnumerator DoRollback()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime * ROLLBACK_SPEED)
        {
            if (actions.Peek().Item1 >= i)
                actions.Dequeue().Item2?.Invoke();
            yield return null;
        }
    }
}
