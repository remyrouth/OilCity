using UnityEngine;

public class OilWellInfoTileAction : InfoTileAction
{
    public override GameObject Create(Transform pivot, float rotation, TileObjectController toc, TileSelector ts, bool upsideDown)
    {
        var obj = Instantiate(prefab, pivot);
        obj.GetComponent<OilWellInfoTileActionView>().Initialize(this, rotation, toc, ts, upsideDown);
        return obj;
    }
}
