using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public sealed class PipeController : BuildingController<BuildingScriptableObject>, IFlowable
{
    // only the child is actually invoked; the parent is more so just for establishing the flow tree
    private IFlowable m_child; // where you get the flow from (the start)
    private IFlowable m_parent; // where the flow goes to (the end)

    private PipeFlowDirection m_startDirection; // the orientation of the start pipe
    private PipeFlowDirection m_endDirection; // the orientation of the end pipe

    private Vector2Int m_startPipePos; // position of the start pipe
    private Vector2Int m_endPipePos; // position of the end pipe

    private List<Vector2Int> m_pipes;

    /// <summary>
    /// Init method for just pipes. Provides necessary values for functionality.
    /// </summary>
    /// <param name="start_pos"></param>
    /// <param name="end_pos"></param>
    /// <param name="start_pipe_dir"></param>
    /// <param name="end_pipe_dir"></param>
    public void InitializePipe(Vector2Int start_pos, Vector2Int end_pos, PipeFlowDirection start_pipe_dir, PipeFlowDirection end_pipe_dir, List<Vector2Int> pipes)
    {
        // notarize all the values passed in
        m_startPipePos = start_pos;
        m_endPipePos = end_pos;

        m_startDirection = start_pipe_dir;
        m_endDirection = end_pipe_dir;

        m_pipes = pipes;
    }

    public void SetTileActions(List<TileAction> actions)
    {
        this.TileActions = actions;
    }

    protected override void CreateInitialConnections(Vector2Int _)
    {
        var child_pos = m_startPipePos + Utilities.GetPipeFlowDirOffset(Utilities.FlipFlow(m_startDirection));
        var parent_pos = m_endPipePos + Utilities.GetPipeFlowDirOffset(m_endDirection);

        var (connect_to_child, connect_to_parent) = ValidatePipesAndConnect(child_pos, parent_pos);

        if (connect_to_child && BoardManager.Instance.TryGetTypeAt<IFlowable>(child_pos, out var obj))
        {
            obj.SetParent(this);
            AddChild(obj);
        }

        if (connect_to_parent && BoardManager.Instance.TryGetTypeAt<IFlowable>(parent_pos, out var pobj))
        {
            pobj.AddChild(this);
            SetParent(pobj);
        }
    }

    /// <summary>
    /// A helper method for testing to see if the pipe connection would be valid. In all cases except for when
    /// connecting two pipes at not endpoints, this returns true/true.
    /// </summary>
    /// <param name="child_end"></param>
    /// <param name="parent_end"></param>
    /// <returns></returns>
    private (bool connect_to_child, bool connect_to_parent) ValidatePipesAndConnect(Vector2Int child_end, Vector2Int parent_end)
    {
        bool is_child_valid = true;
        if (BoardManager.Instance.TryGetTypeAt<PipeController>(child_end, out var c_pipe))
        {
            var (_, end) = c_pipe.GetPositions();

            is_child_valid = end.Equals(child_end);

            if (is_child_valid) c_pipe.UpdateFlowAndVisual(end, m_startPipePos, true);
            // todo include self-update if you soft-link the pipes
        }

        bool is_parent_valid = true;
        if (BoardManager.Instance.TryGetTypeAt<PipeController>(parent_end, out var p_pipe))
        {
            var (start, _) = p_pipe.GetPositions();

            is_parent_valid = start.Equals(parent_end);

            if (is_parent_valid) p_pipe.UpdateFlowAndVisual(start, m_endPipePos, false);
            // todo include self-update if you soft-link the pipes
        }

        return (is_child_valid, is_parent_valid);
    }

    /// <summary>
    /// Called by another pipe controller when this pipe controller needs to update one of its endpoints to
    /// flow into the calling pipe controller.
    /// </summary>
    public void UpdateFlowAndVisual(Vector2Int endpoint, Vector2Int pipe, bool is_flow_in)
    {
        if (!Utilities.GetCardinalEstimatePipeflowDirection(endpoint, pipe, out var flow_direction))
        {
            Debug.LogError(string.Format("Pipeflow not adjacent! {0} {1}", endpoint, pipe));
            return;
        }

        // if the flow is outward, we need to flip the direction we estimated. That was made with the "pipe->tile"
        // assumption, not the "tile->pipe" assumption.
        //if (!is_flow_in)
        //{
        //    flow_direction = Utilities.FlipFlow(flow_direction);
        //    Debug.Log("swpaped");
        //}
        Debug.Log("flow: " + flow_direction);

        var in_pos = Vector2Int.zero;
        var out_pos = Vector2Int.zero;

        // change flowdir for endpoint
        if (endpoint.Equals(m_startPipePos))
        {
            m_startDirection = flow_direction;

            in_pos = pipe;

            // if we're a singleton pipe, just assume that our output position is the opposite axis of our input. no way to know for sure.
            out_pos = m_pipes.Count > 1 ? m_pipes[1] : endpoint + Utilities.GetPipeFlowDirOffset(Utilities.FlipFlow(flow_direction));
        }
        else if (endpoint.Equals(m_endPipePos))
        {
            m_endDirection = flow_direction;

            // if we're a singleton pipe, just assume that our input position is the opposite axis of our output. no way to know for sure.
            in_pos = m_pipes.Count > 1 ? m_pipes[^2] : endpoint + Utilities.GetPipeFlowDirOffset(Utilities.FlipFlow(flow_direction));

            out_pos = pipe;
        }

        // change visual for endpoint
        BoardManager.Instance.ClearSupermapTile(endpoint); // wipe the tile before placing so the transform doesnt get borked
        BoardManager.Instance.SetPipeTileInSupermap(
            endpoint, 
            BuildingManager.Instance.GetPipeRotation(in_pos, endpoint, out_pos));
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
        if (m_child != null && m_child.Equals(child))
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
        m_parent?.DisownChild(this);

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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        // clear all relevant pipe tiles from supermap
        foreach (var pos in m_pipes)
        {
            BoardManager.Instance.ClearSupermapTile(pos);
        }
    }

    /// <summary>
    /// Returns true if the object at the position is the input for this pipe system. i.e. if the tile flows out into the pipe system.
    /// </summary>
    /// <param name="tile_pos"></param>
    /// <returns></returns>
    public bool DoesPipeSystemReceiveInputFromTile(Vector2Int tile_pos)
    {
        if (Utilities.GetCardinalEstimatePipeflowDirection(tile_pos, m_startPipePos, out PipeFlowDirection est_flow_dir)) {
            // flow is flipped here because the estimate flow direction method operates under the assumption that the pipe is always flowing
            // into the tile, not the other way around.
            return Utilities.FlipFlow(est_flow_dir) == m_startDirection;
        }
        else
        {
            // not within any of the cardinal directions, so auto-false.
            return false;
        }
    }

    /// <summary>
    /// Returns true if the object at the position is the output object for this pipe system. i.e. if the pipe system flows into the tile.
    /// </summary>
    /// <param name="tile_pos"></param>
    /// <returns></returns>
    public bool DoesPipeSystemOutputToTile(Vector2Int tile_pos)
    {
        if (Utilities.GetCardinalEstimatePipeflowDirection(tile_pos, m_endPipePos, out PipeFlowDirection est_flow_dir)) return est_flow_dir == m_endDirection;
        else
        {
            // not within any of the cardinal directions, so auto-false.
            return false;
        }
    }

    public (Vector2Int start, Vector2Int end) GetPositions() => (m_startPipePos, m_endPipePos);
}