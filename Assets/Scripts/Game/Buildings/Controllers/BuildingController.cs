using System.Collections.Generic;

public abstract class BuildingController<T> : TileObjectController
    where T : BuildingScriptableObject
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
