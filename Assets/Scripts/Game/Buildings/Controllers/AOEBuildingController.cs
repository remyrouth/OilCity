using System.Collections.Generic;
using UnityEngine;

public abstract class AOEBuildingController : PayrateBuildingController, ITickReceiver
{
    public abstract int TickNumberInterval { get; }
    public virtual int Range =>BaseRange;
    public abstract void OnTick();
    protected int BaseRange { get; private set; }
    public void SetBaseRange(int baseRange) => BaseRange = baseRange;
    protected List<Vector2Int> GetTilesInRange() => BoardManager.Instance.GetTilesInRange(this, Range);
}
