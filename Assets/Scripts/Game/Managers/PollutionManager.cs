using System;
using UnityEngine;

public class PollutionManager : Singleton<PollutionManager>
{
    public float PollutionAmount { get; private set; } = 0;

    public event Action<float> OnPollutionChanged;

    private void Start()
    {
        ChangePollution(0);
    }
    public void ChangePollution(float delta)
    {
        PollutionAmount = Mathf.Clamp01(PollutionAmount + delta);
        OnPollutionChanged?.Invoke(PollutionAmount);
    }

    public void SetPollution(float amount)
    {
        PollutionAmount = Mathf.Clamp01(amount);
        OnPollutionChanged?.Invoke(PollutionAmount);
    }

#if UNITY_EDITOR
    [Range(0, 1)]
    [SerializeField] private float _amountSlider;
    private void OnValidate()
    {
        if (PollutionAmount != _amountSlider)
            ChangePollution(_amountSlider - PollutionAmount);
    }
#endif

}
