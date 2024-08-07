using UnityEngine;

public class RefineryFireEffect : MonoBehaviour,ITickReceiver
{
    private int _timer = 0;
    public void Initialize(int time)
    {
        _timer = time;
        TimeManager.Instance.RegisterReceiver(this);
    }

    public void OnTick()
    {
        if (_timer > 0)
        {
            _timer--;
            return;
        }
        TimeManager.Instance.DeregisterReceiver(this);
        Destroy(gameObject);
    }
}
