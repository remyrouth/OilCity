using System.Collections.Generic;
using UnityEngine;

public class GameDataRecorder : Singleton<GameDataRecorder>, ITickReceiver
{
    [field: SerializeField] public int TickNumberInterval { get; private set; } = 10;
    public readonly List<GameRecord> _records = new();
    private void Start()
    {
        TimeManager.Instance.RegisterReceiver(this);
    }

    private int _tickTimer;
    public void OnTick()
    {
        _tickTimer++;
        if (_tickTimer < TickNumberInterval)
            return;
        _tickTimer = 0;
        TakeSnapshot();
    }
    private void TakeSnapshot()
    {
        GameRecord newRecord = new GameRecord()
        {
            KeroseneAmount = KeroseneManager.Instance.KeroseneAmount,
            MoneyAmount = MoneyManager.Instance.Money,
            WorkerSatisfaction = WorkerSatisfactionManager.Instance.WorkerSatisfaction
        };
        _records.Add(newRecord);
    }

    public class GameRecord
    {
        public float KeroseneAmount;
        public float MoneyAmount;
        public float WorkerSatisfaction;
    }
}
