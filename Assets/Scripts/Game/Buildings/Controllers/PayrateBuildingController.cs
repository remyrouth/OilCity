using System.Collections.Generic;
public abstract class PayrateBuildingController : BuildingController<PayrateBuildingScriptableObject>
{
    protected enum PaymentMode
    {
        LOW,
        MEDIUM,
        HIGH
    }
    

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
