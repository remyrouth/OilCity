using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpriteScript : MonoBehaviour
{
    [Serializable]
    public struct PipeRotation
    {
        public Sprite Sprite;
        public float Rotation;
        public PipeFlowDirection In;
        public PipeFlowDirection Out;

        public readonly bool MatchesFlow(PipeFlowDirection input, PipeFlowDirection output)
        {
            return input.Equals(In) && output.Equals(Out);
        }
    }

    [SerializeField] private List<PipeRotation> m_pipes;

    public PipeRotation OrientPipes(Vector2Int in_pos, Vector2Int pos, Vector2Int out_pos)
    {
        PipeFlowDirection in_dir, out_dir;
        Utilities.GetCardinalEstimatePipeflowDirection(pos, in_pos, out in_dir);
        Utilities.GetCardinalEstimatePipeflowDirection(out_pos, pos, out out_dir);

        bool is_in_invalid = in_dir.Equals(PipeFlowDirection.Invalid);
        bool is_out_invalid = out_dir.Equals(PipeFlowDirection.Invalid);

        if (is_in_invalid && is_out_invalid)
        {
            in_dir = out_dir = PipeFlowDirection.North;
        }
        else if (is_in_invalid)
        {
            in_dir = out_dir;
        }
        else if (is_out_invalid)
        {
            out_dir = in_dir;
        }

        PipeRotation rotation = new();
        foreach (var pipe in m_pipes)
        {
            if (pipe.MatchesFlow(in_dir, out_dir))
            {
                rotation = pipe;
                break;
            }
        }

        if (rotation.Sprite == null)
        {
            Debug.LogError(string.Format("No pipe entry found to match directions in: {0} and out: {1}", in_dir, out_dir));
            return default;
        }

        return rotation;
    }
}
