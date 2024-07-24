using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingController<T> : TileObjectController
    where T : BuildingScriptableObject
{
    [SerializeField] protected List<TileAction> TileActions;
    protected T config;

    /// <summary>
    /// Initialize controller with given configuration
    /// </summary>
    public virtual void Initialize(T config)
    {
        this.config = config;
        CreateInitialConnections(); // for flowables
        TimeManager.Instance.RegisterReceiver(gameObject);
        //setup values

    }
    public override List<TileAction> GetActions() => TileActions;
    protected virtual void OnDestroy()
    {
        TimeManager.Instance.DeregisterReceiver(gameObject);
        //reduce money by config.placementCost
        //reduce satisfaction by config.removalSatisfactionCost
        //play visual effect
    }

    protected virtual void CreateInitialConnections() { }
}
