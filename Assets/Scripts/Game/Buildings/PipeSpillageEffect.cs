using UnityEngine;

public class PipeSpillageEffect : MonoBehaviour
{
    [SerializeField]private ParticleSystem _particleSystem;
    [SerializeField]private Transform _pivot;
    public void Play(Vector2 origin, PipeFlowDirection direction)
    {
        _particleSystem.Play();
        transform.position = origin;

        switch (direction)
        {
            case PipeFlowDirection.North:
                _pivot.transform.eulerAngles = Vector3.zero;
                break;
            case PipeFlowDirection.South:
                _pivot.transform.eulerAngles = Vector3.forward*180;
                break;
            case PipeFlowDirection.East:
                _pivot.transform.eulerAngles = Vector3.forward * 270;
                break;
            case PipeFlowDirection.West:
                _pivot.transform.eulerAngles = Vector3.forward * 90;
                break;
        }
    }
    public void Stop()
    {
        _particleSystem.Stop();
    }
}
