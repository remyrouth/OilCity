using UnityEngine;

public sealed class SiloController : BuildingController<Object>, IFlowable
{
   public float AmountStored { get; private set; }
   public FlowType TypeStored { get; private set; }

    public (FlowType, float) SendFlow()
    {
        throw new System.NotImplementedException();
    }
}
