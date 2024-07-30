using System;
using System.Collections.Generic;
public abstract class PayrateBuildingController : BuildingController<PayrateBuildingScriptableObject>
{
    public event Action PaymentModeIncreased;
    public event Action PaymentModeDecreased;
    protected enum PaymentMode
    {
        LOW,
        MEDIUM,
        HIGH
    }
    protected void IncreasePay()
    {
        switch (paymentMode)
        {
            case PaymentMode.LOW:
                paymentMode = PaymentMode.MEDIUM;
                PaymentModeIncreased?.Invoke();
                break;
            case PaymentMode.MEDIUM:
                paymentMode = PaymentMode.HIGH;
                PaymentModeIncreased?.Invoke();
                break;
            default:
                break;
        }
    }
    protected void DecreasePay() 
    {
        switch (paymentMode)
        {
            case PaymentMode.MEDIUM:
                paymentMode = PaymentMode.LOW;
                PaymentModeDecreased?.Invoke();
                break;
            case PaymentMode.HIGH:
                paymentMode = PaymentMode.MEDIUM;
                PaymentModeDecreased?.Invoke();
                break;
            default:
                break;
        }
    }

    protected abstract void IncreaseProductivity();
    protected abstract void DecreaseProductivity();

    protected PaymentMode paymentMode = PaymentMode.LOW;
    protected void PayWorkers()
    {
        switch (paymentMode)
        {
            case PaymentMode.LOW:
                MoneyManager.Instance.ReduceMoney(config.basePayrate - config.payrateLevelDelta);
                break;
            case PaymentMode.MEDIUM:
                MoneyManager.Instance.ReduceMoney(config.basePayrate);
                break;
            case PaymentMode.HIGH:
                MoneyManager.Instance.ReduceMoney(config.basePayrate + config.payrateLevelDelta);
                break;
        }
    }
    public int GetIndexOfSatisfaction()
    {
        return (int)paymentMode - 1;
    }
}
