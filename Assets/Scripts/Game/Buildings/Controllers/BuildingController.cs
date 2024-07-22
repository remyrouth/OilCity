using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingController<T> : TileObjectController
    where T : Object
{
    protected T config;
    public virtual void Initialize(T config)
    {
        this.config = config;
        //subscribe to TimeManger

        //setup values

    }
    public override List<object> GetActions()
    {
        return base.GetActions();
    }

}
