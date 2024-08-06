using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INewFlowable : ITickReceiver
{
    float SendFlow();

    (bool can_input, bool can_output) GetInOutConfig();

    (FlowType in_type, FlowType out_type) GetFlowConfig();

    TreeRelationship GetTreeRelationship();

    void UpdateConnections(ISet<INewFlowable> seen);
}
