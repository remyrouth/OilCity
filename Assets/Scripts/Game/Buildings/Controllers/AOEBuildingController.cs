using System.Collections.Generic;
using UnityEngine;

public abstract class AOEBuildingController : PayrateBuildingController, ITickReceiver
{
    public abstract int TickNumberInterval { get; }
    public abstract int Range { get; }
    public abstract void OnTick();
    protected List<Vector2Int> GetTilesInRange() => BoardManager.Instance.GetTilesInRange(this, Range);
}
