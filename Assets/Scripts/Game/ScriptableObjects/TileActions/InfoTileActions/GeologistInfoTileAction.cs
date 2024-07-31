using UnityEngine;

public class GeologistInfoTileAction : InfoTileAction
{
    public override GameObject Create(Transform pivot, float rotation, TileObjectController toc, TileSelector ts)
    {
        var obj = Instantiate(prefab, pivot);
        obj.GetComponent<GeologistInfoTileActionView>().Initialize(this, rotation, toc, ts);
        return obj;
    }
}
