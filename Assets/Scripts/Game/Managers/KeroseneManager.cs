using System;

public class KeroseneManager : Singleton<KeroseneManager>
{
    public float KeroseneAmount { get; private set; }
    public float MaxSoldAmount { get; private set; }
    public const float KEROSINE_PRICE = 1;

    public event Action<float> OnKeroseneChanged;
    public event Action OnKeroseneSold;
    
    public void IncreaseAmount(float amount)
    {
        KeroseneAmount += amount;
        OnKeroseneChanged?.Invoke(KeroseneAmount);
    }
    public void DecreaseAmount(float amount)
    {
        if (KeroseneAmount - amount < 0)
            KeroseneAmount = 0;
        else
            KeroseneAmount -= amount;
        OnKeroseneChanged?.Invoke(KeroseneAmount);
    }
    public void SellKerosene()
    {
        MoneyManager.Instance.AddMoney(KEROSINE_PRICE * KeroseneAmount);
        DecreaseAmount(KeroseneAmount);
        OnKeroseneSold?.Invoke();
    }
}
