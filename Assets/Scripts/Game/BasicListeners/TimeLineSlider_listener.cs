using UnityEngine;
using UnityEngine.UI;

public class TimeLineSlider_listener : MonoBehaviour,ITickReceiver
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        TimeManager.Instance.RegisterReceiver(this);
        OnTick();
    }

    public void OnTick()
    {
        _slider.SetValueWithoutNotify(TimeLineEventManager.Instance.GetTimePercentage());
    }

}
