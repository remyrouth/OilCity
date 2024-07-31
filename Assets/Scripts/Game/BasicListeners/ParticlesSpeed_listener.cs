using UnityEngine;

public sealed class ParticlesSpeed_listener : MonoBehaviour
{
    private ParticleSystem _particles;
    private ParticleSystem Particles
    {
        get
        {
            if (_particles == null)
                _particles = GetComponentInChildren<ParticleSystem>();
            return _particles;
        }
    }
    private void Start()
    {
        UpdateSpeed(TimeManager.Instance.TicksPerMinute);
    }
    private void OnEnable()
    {
        TimeManager.Instance.OnTicksPerMinuteChanged += UpdateSpeed;
    }
    private void OnDisable()
    {
        TimeManager.Instance.OnTicksPerMinuteChanged -= UpdateSpeed;
    }
    private void UpdateSpeed(int newTickRate)
    {
        if (newTickRate == 0 && Particles.isPlaying)
            Particles.Pause();
        else
            if (Particles.isPaused && newTickRate != 0)
            Particles.Play();
    }
}
