using System.Collections.Generic;

/// <summary>
/// An interface for items that belong as a node of a tickable tree tree.
/// </summary>
public interface ITreeNode
{
    /// <summary>
    /// Returns a list of all the children of this TreeNode that send flow to this node.
    /// </summary>
    /// <returns>A list of flowables representing all the child nodes that go into this one.</returns>
    List<IFlowable> GetInputChildren();
}
