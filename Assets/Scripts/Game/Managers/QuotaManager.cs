using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuotaManager : Singleton<QuotaManager>
{
    [SerializeField] private float _initialQuota;
    public float NewQuota;
    public float currentQuota;
    public int timeTillQuotaDeadline = 60;
    public event Action<float> OnQuotaChanged;
    void Start()
    {
        currentQuota = _initialQuota;
        OnQuotaChanged?.Invoke(currentQuota);
    }
    public void SetQuota()
    {
        currentQuota = NewQuota;
        OnQuotaChanged?.Invoke(currentQuota);
    }
    public void DecreaseQuota(float newMoneyValue)
    {
        if (currentQuota < newMoneyValue)
            currentQuota = 0;
        else
            currentQuota -= newMoneyValue;
        OnQuotaChanged?.Invoke(currentQuota);
    }
}
