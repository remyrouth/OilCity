using System;
using System.Collections.Generic;

public abstract class PayrateBuildingController : BuildingController<PayrateBuildingScriptableObject>
{
    public event Action OnPaymentModeIncreased;
    public event Action OnPaymentModeDecreased;
    public enum PaymentMode
    {
        LOW,
        MEDIUM,
        HIGH
    }
    public void IncreasePay()
    {
        switch (CurrentPaymentMode)
        {
            case PaymentMode.LOW:
                CurrentPaymentMode = PaymentMode.MEDIUM;
                OnPaymentModeIncreased?.Invoke();
                break;
            case PaymentMode.MEDIUM:
                CurrentPaymentMode = PaymentMode.HIGH;
                OnPaymentModeIncreased?.Invoke();
                break;
            default:
                break;
        }
    }
    public void DecreasePay()
    {
        switch (CurrentPaymentMode)
        {
            case PaymentMode.MEDIUM:
                CurrentPaymentMode = PaymentMode.LOW;
                OnPaymentModeDecreased?.Invoke();
                break;
            case PaymentMode.HIGH:
                CurrentPaymentMode = PaymentMode.MEDIUM;
                OnPaymentModeDecreased?.Invoke();
                break;
            default:
                break;
        }
    }

    protected abstract void IncreaseProductivity();
    protected abstract void DecreaseProductivity();

    public PaymentMode CurrentPaymentMode { get; protected set; } = PaymentMode.MEDIUM;
    protected void PayWorkers()
    {
        switch (CurrentPaymentMode)
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
        return (int)CurrentPaymentMode - 1;
    }
    public override List<TileAction> GetActions() //little hardcoded but u know
    {
        var actions = new List<TileAction>();
        actions.Add(TileActions[0]);
        if (CurrentPaymentMode != PaymentMode.HIGH)
            actions.Add(TileActions[1]);
        actions.Add(TileActions[2]);
        if (CurrentPaymentMode != PaymentMode.LOW)
            actions.Add(TileActions[3]);
        return actions;
    }
}
