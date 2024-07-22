using System.Collections.Generic;

public sealed class OilWellController : PayrateBuildingController, IFlowable
{
    public const float BASE_OIL_RATE = 1;
    public List<IFlowable> GetInputChildren() => null;

    public void OnTick() { }

    public (FlowType type, float amount) SendFlow() => (FlowType.Oil, BASE_OIL_RATE);
}
