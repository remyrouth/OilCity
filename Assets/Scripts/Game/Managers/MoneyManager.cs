using UnityEngine;
using System.Collections;
using System;

public class MoneyManager : Singleton<MoneyManager> {

    public float Money { get; private set; }

    public event Action<float> OnMoneyChanged;

    public bool BuyItem(float cost) {
        if (Money - cost >= 0) {
            ReduceMoney(cost);
            return true;
        } else {
            return false;
        }
    }
    public void AddMoney(float amount) {
        Money = Mathf.Round(Money * 100f) / 100f + Mathf.Round(amount * 100f) / 100f;
        OnMoneyChanged?.Invoke(Money);
    }
    public void ReduceMoney(float amount)
    {
        Money = Mathf.Round(Money * 100f) / 100f - Mathf.Round(amount * 100f) / 100f;
        if (Money < 0)
            gameOver();
        OnMoneyChanged?.Invoke(Money);
    }
    public void gameOver()
    {
        Debug.Log("You lost...");
    }

}