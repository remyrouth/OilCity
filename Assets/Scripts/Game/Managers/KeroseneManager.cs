using System;

public class KeroseneManager : Singleton<KeroseneManager>
{
    public float KeroseneAmount { get; private set; }
    public float MaxSoldAmount;
    public const float KEROSINE_PRICE = 100;

    public event Action<float> OnKeroseneChanged;
    public event Action OnKeroseneSold;

    
    /// <summary>
    /// Increases the current amount of kerosene owned by the player.
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseAmount(float amount)
    {
        KeroseneAmount += amount;
        OnKeroseneChanged?.Invoke(KeroseneAmount);
    }
    /// <summary>
    /// Decreases the current amount of kerosene owned by the player.
    /// </summary>
    /// <param name="amount"></param>
    public void DecreaseAmount(float amount)
    {
        if (KeroseneAmount - amount < 0)
            KeroseneAmount = 0;
        else
            KeroseneAmount -= amount;
        OnKeroseneChanged?.Invoke(KeroseneAmount);
    }
    /// <summary>
    /// Gives the player money for the amount of kerosene sold
    /// </summary>
    public void SellKerosene()
    {
        MoneyManager.Instance.AddMoney(KEROSINE_PRICE * MaxSoldAmount);
        DecreaseAmount(KeroseneAmount);
        OnKeroseneSold?.Invoke();
    }
}
