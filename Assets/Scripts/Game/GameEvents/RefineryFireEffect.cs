using UnityEngine;

public class RefineryFireEffect : MonoBehaviour,ITickReceiver
{
    private int _timer = 0;
    private Vector2Int _refineryPosition;
    private RefineryController _refinery;
    public void Initialize(int time,RefineryController refinery)
    {
        _timer = time;
        TimeManager.Instance.RegisterReceiver(this);
        _refineryPosition = refinery.Anchor;
        _refinery = refinery;
        refinery.underFire = true;
        BoardManager.Instance.OnBuildingDestroyed += CheckIfDestroyed;
    }
    private void CheckIfDestroyed(Vector2Int pos, TileObjectController toc)
    {
        if(pos == _refineryPosition)
            DestroyEffect();
    }
    public void OnTick()
    {
        if (_timer > 0)
        {
            _timer--;
            return;
        }
        DestroyEffect();
    }
    private void DestroyEffect()
    {
        _refinery.underFire = false;
        BoardManager.Instance.OnBuildingDestroyed -= CheckIfDestroyed;
        TimeManager.Instance.DeregisterReceiver(this);
        Destroy(gameObject);
    }
}
