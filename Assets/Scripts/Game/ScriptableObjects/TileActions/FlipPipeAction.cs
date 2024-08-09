using UnityEngine;
public class FlipPipeAction : ClickableTileAction
{
    public override void OnClicked(TileObjectController toc)
    {
        if (toc is PipeController controller)
        {
            controller.FlipPipe();
        }
    }
}