using UnityEngine;

public class GeologistInfoTileAction : InfoTileAction
{
    public override GameObject Create(Transform pivot, float rotation, TileObjectController toc, TileSelector ts,bool upsideDown)
    {
        var obj = Instantiate(prefab, pivot);
        obj.GetComponent<GeologistInfoTileActionView>().Initialize(this, rotation, toc, ts, upsideDown);
        return obj;
    }
}
