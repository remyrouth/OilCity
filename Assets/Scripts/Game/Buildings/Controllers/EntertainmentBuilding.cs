using System.Collections.Generic;
using UnityEngine;

public sealed class EntertainmentBuilding : PayrateBuildingController, ITickReceiver
{

    [SerializeField] private int _workerSatisfactionIncreaseValue;
    [SerializeField] private int _workerSatisfactionDelta;
    private int _tickTimer;
    private int PaymentTimer => 5;
    private bool _isActive = true;



    public void OnTick()
    {
        _tickTimer++;
        if (_tickTimer == PaymentTimer && _isActive)
        {
            _tickTimer = 0;
            WorkerSatisfactionManager.Instance.IncreaseSatisfaction(AmountToIncrease());
            PayWorkers();
        }

    }
    private float AmountToIncrease()
    {
        return (_workerSatisfactionIncreaseValue + ((int)CurrentPaymentMode) * _workerSatisfactionDelta)
            / ((int)Mathf.Sqrt(CivilianCityManager.Instance.NumOfBuildings) + 1);
    }
    public float AmountPerTick() => AmountToIncrease() / PaymentTimer;
    protected override void IncreaseProductivity()
    {
    }

    protected override void DecreaseProductivity()
    {
    }
}
