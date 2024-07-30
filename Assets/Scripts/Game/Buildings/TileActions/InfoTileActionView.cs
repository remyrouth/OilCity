using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InfoTileActionView : SingleTileActionView<TileAction>
{
    [SerializeField] private Image _renderer;
    public override void Initialize(TileAction action, float rotation, TileObjectController toc, TileSelector ts)
    {
        base.Initialize(action, rotation, toc, ts);
        _renderer.sprite = action.GetIcon(toc);
    }
}
