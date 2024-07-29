using System;
using UnityEngine;

public class PollutionManager : MonoBehaviour
{
    public event Action<float> OnPollutionChanged;
    public float PollutionAmount { get; private set; }


    public void ChangePollution(float delta)
    {
        PollutionAmount = Mathf.Clamp01(PollutionAmount+delta);
        OnPollutionChanged?.Invoke(PollutionAmount);
    }

}
