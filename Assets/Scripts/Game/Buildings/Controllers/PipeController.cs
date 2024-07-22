using System.Collections.Generic;
using UnityEngine;

public sealed class PipeController : BuildingController<BuildingScriptableObject>, IFlowable
{
    public List<IFlowable> GetInputChildren()
    {
        throw new System.NotImplementedException();
    }

    public void OnTick()
    {
        throw new System.NotImplementedException();
    }

    public (FlowType, float) SendFlow()
    {
        throw new System.NotImplementedException();
    }
}
