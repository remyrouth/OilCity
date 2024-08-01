using UnityEngine;
using UnityEngine.Rendering;

public class PP_listener : MonoBehaviour
{
    private Volume _volume;
    private void Awake()
    {
        PollutionManager.Instance.OnPollutionChanged += UpdateValue;
    }
    private void UpdateValue(float newValue)
    {
        if (_volume == null)
            _volume = GetComponent<Volume>();
        _volume.weight = newValue;
    }
}
