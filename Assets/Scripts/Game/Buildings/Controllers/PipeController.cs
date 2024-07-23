using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public sealed class PipeController : BuildingController<BuildingScriptableObject>, IFlowable
{
    private enum PipeFlowDirection
    {
        Invalid,
        North,
        South,
        East,
        West
    }

    private IFlowable m_start; // where you get the flow from
    private IFlowable m_end; // where the flow goes to
    private PipeFlowDirection m_startDirection; // the orientation of the start pipe
    private PipeFlowDirection m_endDirection; // the orientation of the end pipe
    private Vector2Int m_startPipePos; // position of the start pipe
    private Vector2Int m_endPipePos; // position of the end pipe

    // TODO init method for JUST PIPES
    /*
     * will need to set the values of the pipeflow and vector2int properties. needs to be a special constructor.
     * 
     * public void MakePipe(int start_pos, int end_pos, start_dir, end_dir);
    */

    public void CreateInitialConnections(Vector2Int size)
    {
        // todo fill out the m_start and m_end fields in this method
        // difficulty with this is that we don't exactly have pipe-laying done, so we don't know when Instantiate will be called.
        // in theory it would be after the end pipe was placed; i.e. after the controller has been made?
        // then does that mean we'll have to call TimeManager register again?! I think we need a custom building controller Instantiate definition for pipes...
    }

    public void AddChild(IFlowable child)
    {
        if (m_start == null || !m_start.Equals(child))
        {
            m_start = child;
        }
        
    }

    public void DisownChild(IFlowable child)
    {
        if (m_start.Equals(child))
        {
            m_start = null;
        }
    }

    public List<IFlowable> GetChildren()
    {
        var singleton_list = new List<IFlowable>
        {
            m_start
        };

        return singleton_list;
    }

    public IFlowable GetParent()
    {
        return m_end;
    }

    public void SetParent(IFlowable parent)
    {
        m_end = parent;
    }

    public void OnTick()
    {
        Debug.LogWarning("Pipe has overflowed " + SendFlow());
    }

    public (FlowType type, float amount) SendFlow()
    {
        return m_start.SendFlow();
    }

    public bool IsInputPipeForTile(Vector2Int tile_pos)
    {
        PipeFlowDirection est_flow_dir;
        if (GetCardinalEstimatePipeflowDirection(tile_pos, m_startPipePos, out est_flow_dir)) return est_flow_dir == m_startDirection;
        else
        {
            // not within any of the cardinal directions, so auto-false.
            return false;
        }
    }

    public bool IsOutputPipeForTile(Vector2Int tile_pos)
    {
        PipeFlowDirection est_flow_dir;
        if (GetCardinalEstimatePipeflowDirection(tile_pos, m_endPipePos, out est_flow_dir)) return est_flow_dir == m_endDirection;
        else
        {
            // not within any of the cardinal directions, so auto-false.
            return false;
        }
    }

    private bool GetCardinalEstimatePipeflowDirection(Vector2Int dest_pos, Vector2Int pipe_pos, out PipeFlowDirection est_flowdir)
    {
        if (pipe_pos.x < dest_pos.x && pipe_pos.y == dest_pos.y)
        {
            est_flowdir = PipeFlowDirection.East;
            return true;
        }
        else if (pipe_pos.x > dest_pos.x && pipe_pos.y == dest_pos.y)
        {
            est_flowdir = PipeFlowDirection.West;
            return true;
        }
        else if (pipe_pos.x == dest_pos.x && pipe_pos.y > dest_pos.y)
        {
            est_flowdir = PipeFlowDirection.South;
            return true;
        }
        else if (pipe_pos.x == dest_pos.x && pipe_pos.y < dest_pos.y)
        {
            est_flowdir = PipeFlowDirection.North;
            return true;
        }

        est_flowdir = PipeFlowDirection.Invalid;
        return false;
    }
}
