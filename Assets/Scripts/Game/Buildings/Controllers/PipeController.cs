using UnityEngine;

public sealed class PipeController : BuildingController<Object>, IFlowable
{
    public (FlowType, float) SendFlow()
    {
        throw new System.NotImplementedException();
    }
}
