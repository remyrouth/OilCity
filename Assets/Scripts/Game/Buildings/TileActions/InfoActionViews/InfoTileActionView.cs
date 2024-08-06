using UnityEngine;

public class InfoTileActionView<T,T1> : SingleTileActionView<T>
    where T : InfoTileAction
    where T1 : TileObjectController
{
    //[SerializeField] private Image _renderer;
    [SerializeField] protected BuildingInfoView<T,T1> _buildingInfoView;
    [SerializeField] private Vector2 _normalPivot, _upsideDownPivot;
    public override void Initialize(T action, float rotation, TileObjectController toc, TileSelector ts, bool upsideDown)
    {
        base.Initialize(action, rotation, toc, ts, upsideDown);
        //_renderer.sprite = action.GetIcon(toc);
        _buildingInfoView.Initialize(action,toc as T1);
        if (upsideDown)
            _buildingInfoView.GetComponent<RectTransform>().anchoredPosition = _upsideDownPivot;
        else
            _buildingInfoView.GetComponent<RectTransform>().anchoredPosition = _normalPivot;
    }
    public override void Deinitialize()
    {
        base.Deinitialize();
        _buildingInfoView.EndFocus();
    }
}
