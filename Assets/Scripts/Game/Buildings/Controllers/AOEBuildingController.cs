using System.Collections.Generic;
using UnityEngine;

public abstract class AOEBuildingController : PayrateBuildingController, ITickReceiver
{
    public abstract int TickNumberInterval { get; }
    public abstract int Range { get; }
    public abstract void OnTick();
    protected List<Vector2Int> GetTilesInRange()
    {
        Vector2Int upperRight = Anchor + config.size;
        List<Vector2Int> tiles = new();

        int range = Range;

        for (int x = Anchor.x - range; x <= upperRight.x + range; x++)
        {
            for (int y = Anchor.y - range; y <= upperRight.y + range; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                int xDistance = Mathf.Max(0, Anchor.x - currentPos.x, currentPos.x - upperRight.x);
                int yDistance = Mathf.Max(0, Anchor.y - currentPos.y, currentPos.y - upperRight.y);

                if (new Vector2Int(xDistance,yDistance).sqrMagnitude<=range*range)
                    tiles.Add(currentPos);
            }
        }

        return tiles;
    }
}
