using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public sealed class PipeController : BuildingController<BuildingScriptableObject>, IFlowable
{

    private IFlowable m_start; // where you get the flow from
    private IFlowable m_end; // where the flow goes to
    private PipeFlowDirection m_startDirection; // the orientation of the start pipe
    private PipeFlowDirection m_endDirection; // the orientation of the end pipe
    private Vector2Int m_startPipePos; // position of the start pipe
    private Vector2Int m_endPipePos; // position of the end pipe

    /// <summary>
    /// Init method for just pipes. Should be called after the full pipe has been laid.
    /// </summary>
    /// <param name="start_pos"></param>
    /// <param name="end_pos"></param>
    /// <param name="start_pipe_dir"></param>
    /// <param name="end_pipe_dir"></param>
    public void MakePipe(Vector2Int start_pos, Vector2Int end_pos, PipeFlowDirection start_pipe_dir, PipeFlowDirection end_pipe_dir)
    {
        // get the positions of where input and output source tiles would be relative to the start and end positions of the pipe
        var child_pos = start_pos + GetPipeFlowDirOffset(start_pipe_dir);
        var parent_pos = end_pos + GetPipeFlowDirOffset(end_pipe_dir);

        if (BoardManager.Instance.IsTileOccupied(child_pos))
        {
            m_start = BoardManager.Instance.tileDictionary[child_pos].GetComponent<IFlowable>();
        }

        if (BoardManager.Instance.IsTileOccupied(parent_pos))
        {
            m_end = BoardManager.Instance.tileDictionary[parent_pos].GetComponent<IFlowable>();
        }
    }

    // todo delete this method?
    public void CreateInitialConnections(Vector2Int size)
    {
        // todo fill out the m_start and m_end fields in this method
        // difficulty with this is that we don't exactly have pipe-laying done, so we don't know when Instantiate will be called.
        // in theory it would be after the end pipe was placed; i.e. after the controller has been made?
        // then does that mean we'll have to call TimeManager register again?! I think we need a custom building controller Instantiate definition for pipes...
    }

    #region tree stuff
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
    #endregion

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

    #region pipe static helper methods
    public static bool GetCardinalEstimatePipeflowDirection(Vector2Int dest_pos, Vector2Int pipe_pos, out PipeFlowDirection est_flowdir)
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

    public static Vector2Int GetPipeFlowDirOffset(PipeFlowDirection direction)
    {
        switch (direction)
        {
            case PipeFlowDirection.North:
                return Vector2Int.up;
            case PipeFlowDirection.South:
                return Vector2Int.down;
            case PipeFlowDirection.East:
                return Vector2Int.right;
            case PipeFlowDirection.West:
                return Vector2Int.left;
            default:
                return Vector2Int.zero;
        }
    }
    #endregion
}

public enum PipeFlowDirection
{
    Invalid,
    North,
    South,
    East,
    West
}