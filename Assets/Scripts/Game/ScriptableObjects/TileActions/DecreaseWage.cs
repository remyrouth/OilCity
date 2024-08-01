public class DecreaseWage : ClickableTileAction
{
    public override void OnClicked(TileObjectController toc)
    {
        var payRated = toc as PayrateBuildingController;
        if (payRated == null)
            return;
        payRated.DecreasePay();
    }
}
