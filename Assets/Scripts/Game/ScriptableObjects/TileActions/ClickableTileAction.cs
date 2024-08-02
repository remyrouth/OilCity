using UnityEngine;

public abstract class ClickableTileAction : TileAction
{
    public abstract void OnClicked(TileObjectController toc);

    public override GameObject Create(Transform pivot, float rotation, TileObjectController toc, TileSelector ts, bool upsideDown)
    {
        var obj = Instantiate(prefab, pivot);
        obj.GetComponent<ClickableTileActionView>().Initialize(this, rotation, toc, ts, upsideDown);
        return obj;
    }
}
