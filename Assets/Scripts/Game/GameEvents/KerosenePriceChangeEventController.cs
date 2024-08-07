using UnityEngine;

public class KerosenePriceChangeEventController : SingleEventController
{
    [SerializeField] private int tickTime;
    [SerializeField] private AnimationCurve multiplier;
    protected override void OnEnable()
    {
        base.OnEnable();
        KeroseneManager.Instance.EnqueuePriceMultiplier(tickTime, multiplier);
    }
}
