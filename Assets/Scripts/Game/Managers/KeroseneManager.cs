using System;
using UnityEngine;

public class KeroseneManager : Singleton<KeroseneManager>
{
    [SerializeField] private AnimationCurve m_falloffCurve;

    public float KeroseneAmount { get; private set; }
    public float MaxSoldAmount;
    public const float KEROSINE_PRICE = 100;

    public event Action<float> OnKeroseneChanged;
    public event Action OnKeroseneSold;


    private float m_falloffPercent;

    public void SetFalloffPercentage(float centage) => m_falloffPercent = centage;

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
        float soldAmount = Mathf.Clamp(KeroseneAmount, 0, MaxSoldAmount);
        MoneyManager.Instance.AddMoney(KEROSINE_PRICE * soldAmount * m_falloffCurve.Evaluate(m_falloffPercent));
        DecreaseAmount(soldAmount);
        OnKeroseneSold?.Invoke();
    }
}
