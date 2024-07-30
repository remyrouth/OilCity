using UnityEngine;
using System;

public class MoneyManager : Singleton<MoneyManager>
{
    public float Money { get; private set; }

    public event Action<float> OnMoneyChanged;
    [SerializeField] private float _initialMoney;
    private void Start()
    {
        Money = _initialMoney;
        OnMoneyChanged += CheckForGameOver;
        OnMoneyChanged?.Invoke(Money);
    }
    public bool BuyItem(float cost)
    {
        if (Money - cost >= 0)
        {
            ReduceMoney(cost);
            return true;
        }
        return false;
    }
    public void AddMoney(float amount)
    {
        Money = Mathf.Round(Money * 100f) / 100f + Mathf.Round(amount * 100f) / 100f;
        OnMoneyChanged?.Invoke(Money);
    }
    public void ReduceMoney(float amount)
    {
        Money = Mathf.Round(Money * 100f) / 100f - Mathf.Round(amount * 100f) / 100f;
        if (Money <= 0)
            GameOver();
        OnMoneyChanged?.Invoke(Money);
    }

    private void CheckForGameOver(float newValue)
    {
        if (newValue <= 0)
            GameOver();
    }
    public void GameOver()
    {
        Debug.Log("Money reached 0!");
        UIStateMachine.Instance.ChangeState(GameState.EndingUI);
    }

}