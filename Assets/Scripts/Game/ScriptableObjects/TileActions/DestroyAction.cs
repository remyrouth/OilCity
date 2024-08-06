using UnityEngine;
public class DestroyAction : ClickableTileAction
{
    public override void OnClicked(TileObjectController toc)
    {
        if (toc.CheckIfDestroyable())
        {
            BoardManager.Instance.Destroy(toc);
            // Debug.Log("Destroy click activated");
            SoundManager.Instance.SelectBuildingDestroySFXTrigger();
        }
    }
}