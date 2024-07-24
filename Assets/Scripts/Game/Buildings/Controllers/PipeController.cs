using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public sealed class PipeController : BuildingController<BuildingScriptableObject>, IFlowable
{
    // only the child is actually invoked; the parent is more so just for establishing the flow tree
    private IFlowable m_child; // where you get the flow from (the start)
    private IFlowable m_parent; // where the flow goes to (the end)

    private PipeFlowDirection m_startDirection; // the orientation of the start pipe
    private PipeFlowDirection m_endDirection; // the orientation of the end pipe

    private Vector2Int m_startPipePos; // position of the start pipe
    private Vector2Int m_endPipePos; // position of the end pipe

    /// <summary>
    /// Init method for just pipes. Provides necessary values for functionality.
    /// </summary>
    /// <param name="start_pos"></param>
    /// <param name="end_pos"></param>
    /// <param name="start_pipe_dir"></param>
    /// <param name="end_pipe_dir"></param>
    public void InitializePipe(Vector2Int start_pos, Vector2Int end_pos, PipeFlowDirection start_pipe_dir, PipeFlowDirection end_pipe_dir)
    {
        // notarize all the values passed in
        m_startPipePos = start_pos;
        m_endPipePos = end_pos;

        m_startDirection = start_pipe_dir;
        m_endDirection = end_pipe_dir;
    }

    protected override void CreateInitialConnections()
    {
        // todo fill out the m_start and m_end fields in this method
        // difficulty with this is that we don't exactly have pipe-laying done, so we don't know when Instantiate will be called.
        // in theory it would be after the end pipe was placed; i.e. after the controller has been made?
        // then does that mean we'll have to call TimeManager register again?! I think we need a custom building controller Instantiate definition for pipes...

        var child_pos = m_startPipePos + Utilities.GetPipeFlowDirOffset(m_startDirection);
        var parent_pos = m_endPipePos + Utilities.GetPipeFlowDirOffset(m_endDirection);

        if (BoardManager.Instance.IsTileOccupied(child_pos))
        {
            m_child = BoardManager.Instance.tileDictionary[child_pos].GetComponent<IFlowable>();
        }

        if (BoardManager.Instance.IsTileOccupied(parent_pos))
        {
            m_parent = BoardManager.Instance.tileDictionary[parent_pos].GetComponent<IFlowable>();
        }
    }

    /// <summary>
    /// Pipes can connect to other pipes.
    /// </summary>
    /// <returns></returns>
    public (bool can_input, bool can_output) GetFlowConfig() => (true, true);

    #region tree stuff
    /// <summary>
    /// If the pipe has no child source or the current child isn't the input child, reassign.
    /// It isn't possible to "add multiple" children to a pipe because there's only one connection.
    /// </summary>
    /// <param name="child"></param>
    public void AddChild(IFlowable child)
    {
        if (m_child == null || !m_child.Equals(child))
        {
            m_child = child;
        }
        
    }

    /// <summary>
    /// If the given child is equal to the current child, dereference them.
    /// </summary>
    /// <param name="child"></param>
    public void DisownChild(IFlowable child)
    {
        if (m_child.Equals(child))
        {
            m_child = null;
        }
    }

    /// <summary>
    /// Returns a singleton list of the child this pipe sources input from. The list can be modified with no
    /// affect on the pipe itself. Use Add/DisownChild if you need to do that.
    /// 
    /// Returns an empty list if the child is null.
    /// </summary>
    /// <returns></returns>
    public List<IFlowable> GetChildren()
    {
        if (m_child == null) return new List<IFlowable>();

        var singleton_list = new List<IFlowable>
        {
            m_child
        };

        return singleton_list;
    }

    /// <summary>
    /// Returns the parent of the pipe; i.e. the destination of the flow from the child.
    /// </summary>
    /// <returns></returns>
    public IFlowable GetParent()
    {
        return m_parent;
    }

    /// <summary>
    /// Sets the flow destination to the given parent.
    /// </summary>
    /// <param name="parent"></param>
    public void SetParent(IFlowable parent)
    {
        m_parent = parent;
    }
    #endregion

    /// <summary>
    /// TODO implement more, but for now, just recurses down the tree and reports the amount gathered as overflow. This method is invoked
    /// directly if a pipe becomes a member of the tickable forest.
    /// </summary>
    public void OnTick()
    {
        Debug.LogWarning("Pipe has overflowed " + SendFlow());
    }

    /// <summary>
    /// For a pipe's sendflow method, they just report the flow of their source.
    /// </summary>
    /// <returns></returns>
    public (FlowType type, float amount) SendFlow()
    {
        if (m_child == null) return (FlowType.None, 0f);

        return m_child.SendFlow();
    }

    /// <summary>
    /// Returns true if the object at the position is the input for this pipe system.
    /// </summary>
    /// <param name="tile_pos"></param>
    /// <returns></returns>
    public bool IsInputPipeForTile(Vector2Int tile_pos)
    {
        if (Utilities.GetCardinalEstimatePipeflowDirection(tile_pos, m_startPipePos, out PipeFlowDirection est_flow_dir)) return est_flow_dir == m_startDirection;
        else
        {
            // not within any of the cardinal directions, so auto-false.
            return false;
        }
    }

    /// <summary>
    /// Returns true if the object at the position is the output object for this pipe system.
    /// </summary>
    /// <param name="tile_pos"></param>
    /// <returns></returns>
    public bool IsOutputPipeForTile(Vector2Int tile_pos)
    {
        if (Utilities.GetCardinalEstimatePipeflowDirection(tile_pos, m_endPipePos, out PipeFlowDirection est_flow_dir)) return est_flow_dir == m_endDirection;
        else
        {
            // not within any of the cardinal directions, so auto-false.
            return false;
        }
    }
}