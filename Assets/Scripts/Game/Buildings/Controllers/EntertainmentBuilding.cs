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
            if (CurrentPaymentMode == PaymentMode.MEDIUM)
                WorkerSatisfactionManager.Instance.IncreaseSatisfaction(_workerSatisfactionIncreaseValue);
            else if (CurrentPaymentMode == PaymentMode.HIGH)
                WorkerSatisfactionManager.Instance.IncreaseSatisfaction(_workerSatisfactionIncreaseValue + _workerSatisfactionDelta);
            PayWorkers();
        }
        
    }

    protected override void IncreaseProductivity()
    {
        if (CurrentPaymentMode != PaymentMode.MEDIUM) return;
        _isActive = true;
    }

    protected override void DecreaseProductivity()
    {
        if (CurrentPaymentMode != PaymentMode.LOW) return;
        _isActive = false;
    }
}
