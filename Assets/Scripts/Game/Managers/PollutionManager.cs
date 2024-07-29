using System;
using UnityEngine;

public class PollutionManager : Singleton<PollutionManager>
{
    public float PollutionAmount { get; private set; } = 0

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

}
