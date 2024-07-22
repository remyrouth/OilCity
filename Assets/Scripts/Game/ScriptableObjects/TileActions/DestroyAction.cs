public class DestroyAction : TileAction
{
    public override void OnClicked(TileObjectController toc)
    {
        BoardManager.Instance.Destroy(toc);
    }
}