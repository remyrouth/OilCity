using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class KeroseneManager : Singleton<KeroseneManager>, ITickReceiver
{
    [SerializeField] private AnimationCurve m_falloffCurve;

    public float KeroseneAmount { get; private set; }
    public float KeroseneSumAmount { get; private set; } = 0;
    public float MaxSoldAmount;
    public const float KEROSINE_PRICE = 100;

    public event Action<float> OnKeroseneChanged;
    public event Action OnKeroseneSold;

    private List<PriceEffect> effects = new();


    /// <summary>
    /// Increases the current amount of kerosene owned by the player.
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseAmount(float amount)
    {
        KeroseneAmount += amount * 100;
        KeroseneSumAmount += amount * 100;
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
        MoneyManager.Instance.AddMoney(GetKerosenePrice() * soldAmount);
        DecreaseAmount(soldAmount);
        OnKeroseneSold?.Invoke();
    }
    public float GetKerosenePrice()
    {
        float multiplier = m_falloffCurve.Evaluate(TimeLineEventManager.Instance.GetTimePercentage());
        foreach (var effect in effects)
            multiplier *= effect.multiplier.Evaluate((float)effect.timeElapsed/effect.length);
        return KEROSINE_PRICE * multiplier;
    }
    public void EnqueuePriceMultiplier(int time, AnimationCurve multiplier)
    {
        effects.Add(new PriceEffect() { length = time, multiplier = multiplier });
    }

    public void OnTick()
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            effects[i].timeElapsed++;
            if (effects[i].timeElapsed < effects[i].length)
                effects.RemoveAt(i);
        }
    }

    private class PriceEffect { public int length; public AnimationCurve multiplier; public int timeElapsed = 0; }
}
