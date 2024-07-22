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
    /// Returns true if the given gameobject is a flowable and relevant to this flowable (i.e. if the given object is a refinery
    /// and we are a pipe, return true if we are piping into/out of the refinery).
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    bool IsPipeConnected(GameObject other); 
}
