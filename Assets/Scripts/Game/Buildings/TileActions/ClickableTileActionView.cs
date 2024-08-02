using UnityEngine;
using UnityEngine.UI;

public class ClickableTileActionView : SingleTileActionView<ClickableTileAction>
{
    [SerializeField] private Image _renderer;
    [SerializeField] private Button _button;
    public override void Initialize(ClickableTileAction action, float rotation, TileObjectController toc, TileSelector ts, bool upsideDown)
    {
        base.Initialize(action, rotation, toc, ts, upsideDown);
        _renderer.sprite = action.GetIcon(toc);
        _button.onClick.AddListener(() => { action.OnClicked(toc); ts.EndFocus(); });
    }
}
