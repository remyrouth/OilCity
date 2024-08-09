using UnityEngine;

public class SkyEffects_listener : MonoBehaviour, ITickReceiver
{
    private void Start()
    {
        TimeManager.Instance.RegisterReceiver(this);
    }
    public void OnTick()
    {
        transform.localEulerAngles = Vector3.forward * Random.Range(0f, 360f);
    }
}
