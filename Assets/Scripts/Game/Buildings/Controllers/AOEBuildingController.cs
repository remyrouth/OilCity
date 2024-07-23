using System.Collections.Generic;
using UnityEngine;

public abstract class AOEBuildingController : PayrateBuildingController, ITickReceiver
{
    public abstract int TickNumberInterval { get; }
    public abstract int Range { get; }

    private int _tickTimer = 0;
    public void OnTick()
    {
        _tickTimer++;
        if (_tickTimer > TickNumberInterval)
        {
            _tickTimer = 0;
            InvokeAction();
        }
    }
    /// <summary>
    /// Called repeatedly after 'TickNumberInterval' number of ticks
    /// </summary>
    protected abstract void InvokeAction();
    /// <summary>
    /// Calculate Set of tiles withing range taking building size into account
    /// </summary>
    /// <returns></returns>
    protected HashSet<Vector2Int> GetTilesInRange()
    {
        Vector2Int anchor = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int upperRight = anchor + config.size;
        HashSet<Vector2Int> tiles = new();

        int range = Range;


        for (int x = anchor.x - range; x <= upperRight.x + range; x++)
        {
            for (int y = anchor.y - range; y <= upperRight.y + range; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                int xDistance = Mathf.Max(0, anchor.x - currentPos.x, currentPos.x - upperRight.x);
                int yDistance = Mathf.Max(0, anchor.y - currentPos.y, currentPos.y - upperRight.y);

                if (xDistance <= range && yDistance <= range)
                    tiles.Add(currentPos);
            }
        }

        return tiles;
    }
}
