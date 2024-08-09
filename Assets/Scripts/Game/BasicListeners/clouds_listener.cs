using UnityEngine;

public class clouds_listener : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private float _endEmmision;
    [SerializeField] private Color _endColor;
    private float _startEmmision;
    private Color _startColor;
    private void Awake()
    {
        PollutionManager.Instance.OnPollutionChanged += UpdateValue;
        var em = _particle.emission;
        _startEmmision = em.rateOverTimeMultiplier;
        var main = _particle.main;
        _startColor = main.startColor.color;
    }
    private void UpdateValue(float newValue)
    {
        try
        {
            var emissionModule = _particle.emission;
            emissionModule.rateOverTimeMultiplier = Mathf.Lerp(_startEmmision, _endEmmision, newValue);
            var main = _particle.main;
            main.startColor = Color.Lerp(_startColor, _endColor, newValue);
        }
        catch { }
    }
}
