using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeroseneManager : Singleton<KeroseneManager>
{
    private int _tickTimer = 0;
    public int tickNumberInterval = 15;
    public float keroseneAmount { get; private set; }
    public float maxSoldAmount { get; private set; }
    public float minKeroseneAmount => 0;
    public float kerosenePricePerLiter;
    public void OnTick()
    {        
        _tickTimer++;
        if (_tickTimer > tickNumberInterval)
        {
            _tickTimer = 0;
            InvokeAction();
        }
    }
    public void InvokeAction()
    {
        if(keroseneAmount > minKeroseneAmount)
            SellKerosene();
    }
    public void IncreaseAmount(float amount)
    {
        keroseneAmount += amount;
    }
    public void DecreaseAmount(float amount)
    {
        if (keroseneAmount - amount < minKeroseneAmount)
            keroseneAmount = minKeroseneAmount;
        else
            keroseneAmount -= amount;
    }
    public void SellKerosene()
    {
        MoneyManager.Instance.AddMoney(kerosenePricePerLiter * keroseneAmount);
        DecreaseAmount(keroseneAmount);
    }
}
