using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingController<T> : TileObjectController
    where T : BuildingScriptableObject
{
    [SerializeField] protected List<TileAction> TileActions; 
    protected T config;
    public virtual void Initialize(T config)
    {
        this.config = config;
        //subscribe to TimeManger
        
        //setup values

    }
    public override List<TileAction> GetActions() => TileActions;
    protected virtual void OnDestroy()
    {
        //unsubscribe from TimeManager
        //reduce money by config.placementCost
        //reduce satisfaction by config.removalSatisfactionCost
        //play visual effect
    }
}
