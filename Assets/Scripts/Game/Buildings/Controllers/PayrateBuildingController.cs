using System;
using System.Collections.Generic;
using UnityEngine;

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
        float amountToPay = config.basePayrate + ((int)CurrentPaymentMode) * config.payrateLevelDelta;
        float leftToPay = Mathf.Clamp(amountToPay - MoneyManager.Instance.Money, 0, amountToPay);
        MoneyManager.Instance.ReduceMoney(amountToPay - leftToPay);

        if (leftToPay > 0)
        {
            
            //PopupValuesPool.Instance.GetFromPool<SimpleIconPopup>(PopupValuesPool.PopupValueType.MadPeople)
            //.Initialize(ActionsPivot + Vector2.right / 2);
            WorkerSatisfactionManager.Instance.DecreaseSatisfaction((int)(leftToPay / amountToPay * 10));
        }
        PopupValuesPool.Instance.GetFromPool<SimpleTextPopup>(PopupValuesPool.PopupValueType.PaidMoney)
            .Initialize(((int)(leftToPay-amountToPay)).ToString(), ActionsPivot);
    }
    public int GetIndexOfSatisfaction()
    {
        return (int)CurrentPaymentMode - 1;
    }
    public override List<TileAction> GetActions() //little hardcoded but u know
    {
        var actions = new List<TileAction>();
        actions.Add(TileActions[0]);
        actions.Add(TileActions[1]);
        if (CurrentPaymentMode != PaymentMode.HIGH)
            actions.Add(TileActions[2]);
        if (CurrentPaymentMode != PaymentMode.LOW)
            actions.Add(TileActions[3]);
        return actions;
    }
}
