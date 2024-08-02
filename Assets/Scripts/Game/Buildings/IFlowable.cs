using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// An interface for classes that should implement the ability to send flows of oil/kerosene to another object. All classes that do 
/// so need to know the flows incoming from other sources in addition to having the ability to respond to game ticks (since that is what
/// initiates the flow in the first place).
/// </summary>
public interface IFlowable : ITreeNode, ITickReceiver
{
    /// <summary>
    /// Computes and returns the amount of flow for a type of flow.
    /// </summary>
    /// <returns>A tuple with the first item being the type of flow conferred and the second item being the amount of flow.</returns>
    (FlowType type, float amount) SendFlow();

    /// <summary>
    /// Can this flowable receive/send flow from/to a pipe?
    /// </summary>
    /// <returns></returns>
    (bool can_input, bool can_output) GetFlowConfig();
}
