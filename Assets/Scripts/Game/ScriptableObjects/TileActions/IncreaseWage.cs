public class IncreaseWage : ClickableTileAction
{
    public override void OnClicked(TileObjectController toc)
    {
        var payRated = toc as PayrateBuildingController;
        if (payRated == null)
            return;
        payRated.IncreasePay();
    }
}
