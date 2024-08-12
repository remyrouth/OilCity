using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingController<T> : TileObjectController
    where T : BuildingScriptableObject
{
    [SerializeField] protected List<TileAction> TileActions;
    [SerializeField] private Transform _demolishEffect;
    public T config { get; protected set; }
    public override BuildingScriptableObject Config => config;
    public override Vector2Int size => config.size;
    public override Action GetCreateAction(Vector2Int pos) => ()=>config.CreateInstance(pos);
    /// <summary>
    /// Initialize controller with given configuration
    /// </summary>
    public virtual void Initialize(T config, Vector2Int spawn_position)
    {
        this.config = config;
        CreateInitialConnections(spawn_position); // for flowables
        TimeManager.Instance.RegisterReceiver(this);
        //setup values

    }
    public override List<TileAction> GetActions() => TileActions;
    protected virtual void OnDestroy()
    {
        MakeDestroyEffect();
        WorkerSatisfactionManager.Instance.DecreaseSatisfaction(config.removalSatisfactionCost);
        MoneyManager.Instance.ReduceMoney(config.removalCost);
        TimeManager.Instance.DeregisterReceiver(this);
    }
    public override bool CheckIfDestroyable()
    {
        return MoneyManager.Instance.Money >= config.removalCost;
    }
    protected virtual void CreateInitialConnections(Vector2Int with_position) { }
    protected virtual void MakeDestroyEffect()
    {
        if (_demolishEffect == null)
            return;
        _demolishEffect.SetParent(null);
        var particleSystem = _demolishEffect.GetComponentInChildren<ParticleSystem>();
        particleSystem.Play();
        Destroy(_demolishEffect.gameObject, particleSystem.main.duration);
    }
}
