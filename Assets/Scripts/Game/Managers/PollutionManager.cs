using System;
using System.Linq;
using UnityEngine;

public class PollutionManager : Singleton<PollutionManager>, ITickReceiver
{
    public float PollutionAmount { get; private set; } = 0;

    public event Action<float> OnPollutionChanged;

    private void Start()
    {
        TimeManager.Instance.RegisterReceiver(this);
        SetPollution(0);
    }
    public void CalculateNewPollution()
    {
        var timePercentage = Mathf.Clamp01(TimeLineEventManager.Instance.GetTimePercentage());

        var amountOfRafineries = BoardManager.Instance.tileDictionary.Values
            .OfType<RefineryController>().Distinct().Count();
        var amountOfOilWells = BoardManager.Instance.tileDictionary.Values
                    .OfType<OilWellController>().Distinct().Count();

        float pollution = Mathf.Clamp01(timePercentage * 0.75f
            + (float)(amountOfOilWells + amountOfRafineries) / 100
            + Mathf.Clamp(KeroseneManager.Instance.KeroseneSumAmount / 50, 0, 0.1f));

        SetPollution(pollution);
    }

    private void SetPollution(float amount)
    {
        PollutionAmount = Mathf.Clamp01(amount);
        OnPollutionChanged?.Invoke(PollutionAmount);
    }
    public void OnTick()
    {
        CalculateNewPollution();
    }

#if UNITY_EDITOR
    [Range(0, 1)]
    [SerializeField] private float _amountSlider;
    private void OnValidate()
    {
        if (PollutionAmount != _amountSlider)
            SetPollution(_amountSlider);
    }
#endif

}
