using System.Collections.Generic;
using UnityEngine;

public sealed class RefineryController : PayrateBuildingController, IFlowable
{
    public const float BASE_REFINERY_FLOWRATE = 1;
    public List<IFlowable> GetInputChildren() => null;

    public void OnTick() { }

    public (FlowType type, float amount) SendFlow()
    {
        float OilSum = 0;
        foreach (var child in GetInputChildren())
        {
            var received = child.SendFlow();
            if (received.type == FlowType.Kerosene)
            {
                Debug.LogWarning("Refinery just received Kerosene!!!",gameObject);
                continue;
            }
            OilSum += received.amount;
        }
        if (OilSum > BASE_REFINERY_FLOWRATE)
        {
            float diff = OilSum - BASE_REFINERY_FLOWRATE;
            Debug.LogWarning($"Spilled {diff} amount of Kerosene!", gameObject);
            OilSum = BASE_REFINERY_FLOWRATE;
        }
        return (FlowType.Kerosene, OilSum);
    }
}
