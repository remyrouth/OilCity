using UnityEngine;

public class KerosenePriceChangeEventController : SingleEventController
{
    [SerializeField] private int tickTime;
    [SerializeField] private AnimationCurve multiplier;
    protected void OnEnable()
    {
        KeroseneManager.Instance.EnqueuePriceMultiplier(tickTime, multiplier);
    }
}
