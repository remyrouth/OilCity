public class DestroyAction : ClickableTileAction
{
    public override void OnClicked(TileObjectController toc)
    {
        if (toc.CheckIfDestroyable())
        {
            BoardManager.Instance.Destroy(toc);
        }
    }
}