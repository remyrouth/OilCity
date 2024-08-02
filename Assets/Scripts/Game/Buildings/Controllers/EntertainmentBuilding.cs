using System.Collections.Generic;
using UnityEngine;

public sealed class EntertainmentBuilding : PayrateBuildingController, ITickReceiver
{
    
    [SerializeField] private int _workerSatisfactionIncreaseValue;
    [SerializeField] private int _workerSatisfactionDelta;
    public int CurrentSatisfactionIncreaseValue;
    private int _tickTimer;
    private int PaymentTimer => 5;
    private bool _isActive = true;


    public void Start()
    {
         ChangeCurrentSatisfactionValue();

    }

    public void OnTick()
    {
        _tickTimer++;
        if (_tickTimer == PaymentTimer && _isActive)
        {
            ChangeCurrentSatisfactionValue();
            _tickTimer = 0;
            if (CurrentPaymentMode == PaymentMode.MEDIUM)
                WorkerSatisfactionManager.Instance.IncreaseSatisfaction(CurrentSatisfactionIncreaseValue);
            else if (CurrentPaymentMode == PaymentMode.HIGH)
                WorkerSatisfactionManager.Instance.IncreaseSatisfaction(CurrentSatisfactionIncreaseValue);
            PayWorkers();
        }
        
    }

    protected override void IncreaseProductivity()
    {
        if (CurrentPaymentMode != PaymentMode.MEDIUM) return;
        ChangeCurrentSatisfactionValue();
        _isActive = true;
    }

    protected override void DecreaseProductivity()
    {
        if (CurrentPaymentMode != PaymentMode.LOW) return;
        ChangeCurrentSatisfactionValue();
        _isActive = false;
    }
    private void ChangeCurrentSatisfactionValue()
    {
        switch (CurrentPaymentMode)
        {
            case PaymentMode.MEDIUM:
                CurrentSatisfactionIncreaseValue = _workerSatisfactionIncreaseValue;
                break;
            case PaymentMode.HIGH:
                CurrentSatisfactionIncreaseValue = _workerSatisfactionIncreaseValue + _workerSatisfactionDelta;
                break;
            default:
                break;
        }
    }
}
