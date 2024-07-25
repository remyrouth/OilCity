using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingController<T> : TileObjectController
    where T : BuildingScriptableObject
{
    [SerializeField] protected List<TileAction> TileActions;
    protected T config;
    public override Vector2Int size => config.size;
    /// <summary>
    /// Initialize controller with given configuration
    /// </summary>
    public virtual void Initialize(T config)
    {
        this.config = config;
        CreateInitialConnections();
        TimeManager.Instance.RegisterReceiver(gameObject);
        //setup values

    }
    public override List<TileAction> GetActions() => TileActions;
    protected virtual void OnDestroy()
    {
        TimeManager.Instance.DeregisterReceiver(gameObject);
        WorkerSatisfactionManager.Instance.DecreaseSatisfaction(config.removalSatisfactionCost);
        MoneyManager.Instance.ReduceMoney(config.removalCost);
        //play visual effect
    }
    public override bool CheckIfDestroyable()
    {
        return MoneyManager.Instance.money >= config.removalCost;
    }
    protected virtual void CreateInitialConnections() { }
}
