using System.Collections.Generic;

/// <summary>
/// An interface for items that belong as a node of a tickable tree tree. Deals in Flows because otherwise accessing
/// the main sendflow method wouldn't work. Flowables extend TreeNodes anyways, so it's chill.
/// </summary>
public interface ITreeNode
{
    /// <summary>
    /// Gets all the children of the node.
    /// </summary>
    /// <returns></returns>
    List<IFlowable> GetChildren();

    /// <summary>
    /// Gets the parent of the node. A node can only have one parent, bc a building can only have 1 output. (TBD add more?)
    /// </summary>
    /// <returns></returns>
    IFlowable GetParent();

    /// <summary>
    /// Adds the given flowable as a child.
    /// </summary>
    /// <param name="child"></param>
    void AddChild(IFlowable child);

    /// <summary>
    /// Removes the given flowable from this object's child representation, if it's in there
    /// </summary>
    /// <param name="child"></param>
    void DisownChild(IFlowable child);

    /// <summary>
    /// Sets this node's parent to the given flowable.
    /// </summary>
    /// <param name="parent"></param>
    void SetParent(IFlowable parent);
}
