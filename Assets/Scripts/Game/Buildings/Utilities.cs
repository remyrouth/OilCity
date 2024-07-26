using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public static PipeFlowDirection FlipFlow(PipeFlowDirection direction)
    {
        switch (direction)
        {
            case PipeFlowDirection.North: return PipeFlowDirection.South;
            case PipeFlowDirection.South: return PipeFlowDirection.North;
            case PipeFlowDirection.West: return PipeFlowDirection.East;
            case PipeFlowDirection.East: return PipeFlowDirection.West;
            default:
                throw new ArgumentException("Invalid pipes cannot be flipped.");
        }
    }

    /// <summary>
    /// Given a position for tile and pipe, returns true if they are adjacent in a cardinal direction. Additionally, outputs the potential flow
    /// direction for the pipe going into the tile. Note that this only gets the flow for pipes that are flowing outward into a tile. If you want
    /// the flow of a tile into a pipe, you will need to flip the result of this method call.
    /// </summary>
    /// <param name="dest_pos"></param>
    /// <param name="pipe_pos"></param>
    /// <param name="est_flowdir"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Converts the given vector3 into a vector2int using the x and y values floored to integers.
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2Int Vector3ToVector2Int(Vector3 vec) => new(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));


    /// <summary>
    /// Converts the given vector2int into a vector3 using the x and y values.
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector3 Vector2IntToVector3(Vector2Int vec) => new(vec.x, vec.y);
}
