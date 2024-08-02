using UnityEngine;

public class RefineryInfoTileAction : InfoTileAction
{
    public override GameObject Create(Transform pivot, float rotation, TileObjectController toc, TileSelector ts, bool upsideDown)
    {
        var obj = Instantiate(prefab, pivot);
        obj.GetComponent<RefineryInfoTileActionView>().Initialize(this, rotation, toc, ts, upsideDown);
        return obj;
    }
}
