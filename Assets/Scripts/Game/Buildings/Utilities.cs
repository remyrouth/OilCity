using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
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
    public static Vector2Int Vector3ToVector2Int(Vector3 vec) => new(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.x));


    /// <summary>
    /// Converts the given vector2int into a vector3 using the x and y values.
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector3 Vector2IntToVector3(Vector3 vec) => new(vec.x, vec.y);
}
