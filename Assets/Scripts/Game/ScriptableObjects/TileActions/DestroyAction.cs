public class DestroyAction : TileAction
{
    public override void OnClicked(TileObjectController toc)
    {
        if (toc.CheckIfDestroyable())
        {
            BoardManager.Instance.Destroy(toc);
        }
    }
}