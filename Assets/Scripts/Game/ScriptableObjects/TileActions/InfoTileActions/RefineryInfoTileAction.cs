using UnityEngine;

public class RefineryInfoTileAction : InfoTileAction
{
    public override GameObject Create(Transform pivot, float rotation, TileObjectController toc, TileSelector ts)
    {
        var obj = Instantiate(prefab, pivot);
        obj.GetComponent<RefineryInfoTileActionView>().Initialize(this, rotation, toc, ts);
        return obj;
    }
}
