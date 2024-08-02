using UnityEngine;
public class WageTileInfo : TileAction
{
    public override Sprite GetIcon(TileObjectController toc)
    {
        var payRated = toc as PayrateBuildingController;
        if (payRated == null)
            return null;
        return WageIcons[(int)payRated.CurrentPaymentMode];
    }
    [field: SerializeField] public Sprite[] WageIcons { get; protected set; }
    public override GameObject Create(Transform pivot, float rotation, TileObjectController toc, TileSelector ts)
    {
        return base.Create(pivot, rotation, toc, ts);
    }
}
