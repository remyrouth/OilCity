using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class NewPipeController : BuildingController<BuildingScriptableObject>, INewFlowable
{
    private PipeFlowDirection m_lhsFlowDir;
    private PipeFlowDirection m_rhsFlowDir;

    private PipeFlowDirection m_lhsOffsetDir;
    private PipeFlowDirection m_rhsOffsetDir;

    private TreeRelationship m_tr;

    private IReadOnlyList<Vector2Int> m_allPipes;

    private int m_lhsIndex;
    private int m_rhsIndex;

    private Vector2Int m_lhsConnectionPos;
    private Vector2Int m_rhsConnectionPos;

    // VFX

    [SerializeField] private PipeSpillageEffect _oilSpillout, _keroseneSpillout;
#if UNITY_EDITOR
    [SerializeField] private Mesh m_debugMesh;
#endif

    public (FlowType in_type, FlowType out_type) GetFlowConfig()
    {
        FlowType in_type = FlowType.Ambiguous;
        FlowType out_type = FlowType.Ambiguous;

        if (m_tr.HasMaxParents())
        {
            out_type = m_tr.GetParents()[0].GetFlowConfig().in_type;
        }

        if (m_tr.HasMaxChildren())
        {
            in_type = m_tr.GetChildren()[0].GetFlowConfig().out_type;
        }

        if (out_type != in_type && out_type != FlowType.Ambiguous && in_type != FlowType.Ambiguous)
        {
            throw new System.InvalidOperationException("Pipe flow config invalid!");
        }

        return (in_type, out_type);
    }

    public (bool can_input, bool can_output) GetInOutConfig()
    {
        bool can_input = true;
        bool can_output = true;

        if (m_tr.HasMaxParents())
        {
            can_input = m_tr.GetParents()[0].GetInOutConfig().can_input;
        }

        if (m_tr.HasMaxChildren())
        {
            can_output = m_tr.GetChildren()[0].GetInOutConfig().can_output;
        }

        return (can_input, can_output);
    }

    public void InitializePipe(IReadOnlyList<Vector2Int> pipes, Vector2Int start, Vector2Int end)
    {
        m_allPipes = pipes;

        m_lhsIndex = 0;
        m_rhsIndex = pipes.Count - 1;
        for (int i = 0; i < pipes.Count; i++)
        {
            if (pipes[i].Equals(start)) m_lhsIndex = i;
            if (pipes[i].Equals(end)) m_rhsIndex = i;
        }

        if (m_lhsIndex > 0)
        {
            Utilities.GetCardinalEstimatePipeflowDirection(m_allPipes[m_lhsIndex], m_allPipes[m_lhsIndex - 1], out m_lhsOffsetDir);
            m_lhsOffsetDir = Utilities.FlipFlow(m_lhsOffsetDir); // this will be flipped
        }
        else
        {
            Utilities.GetCardinalEstimatePipeflowDirection(m_allPipes[0], m_allPipes[1], out m_lhsOffsetDir);
        }

        if (m_rhsIndex < m_allPipes.Count - 1)
        {
            Utilities.GetCardinalEstimatePipeflowDirection(m_allPipes[m_rhsIndex], m_allPipes[m_rhsIndex + 1], out m_rhsOffsetDir);
            m_rhsOffsetDir = Utilities.FlipFlow(m_rhsOffsetDir);
        }
        else
        {
            Utilities.GetCardinalEstimatePipeflowDirection(m_allPipes[^1], m_allPipes[^2], out m_rhsOffsetDir);
        }

        m_tr = new TreeRelationship(1, 1);

        m_lhsConnectionPos = m_allPipes[m_lhsIndex] + Utilities.GetPipeFlowDirOffset(m_lhsOffsetDir);
        m_rhsConnectionPos = m_allPipes[m_rhsIndex] + Utilities.GetPipeFlowDirOffset(m_rhsOffsetDir);
    }

    protected override void CreateInitialConnections(Vector2Int _)
    {
        NewPipeController lhs = null, rhs = null;
        var pipes = TryGetPipeConnections(ref lhs, ref rhs);

        // stands for "possible lhs" and "possible rhs"
        INewFlowable p_lhs;
        INewFlowable p_rhs;
        if (pipes.lhs) p_lhs = lhs;
        else BoardManager.Instance.TryGetTypeAt(m_lhsConnectionPos, out p_lhs);

        if (pipes.rhs) p_rhs = rhs;
        else BoardManager.Instance.TryGetTypeAt(m_rhsConnectionPos, out p_rhs);

        bool lhs_exists = p_lhs != null;
        bool rhs_exists = p_rhs != null;

        // if two parents exist, check validity of connection and connect them
        if (lhs_exists && rhs_exists) Connect(p_lhs, p_rhs);
        else if (lhs_exists) ConnectSingleSide(p_lhs);
        else if (rhs_exists) ConnectSingleSide(p_rhs);
    }

    private (bool lhs, bool rhs) TryGetPipeConnections(ref NewPipeController lhs_controller, ref NewPipeController rhs_controller)
    {
        var connections = GetConnectionPositions();
        bool has_lhs = false;
        bool has_rhs = false;

        if (BoardManager.Instance.TryGetTypeAt(connections[0], out lhs_controller) && !lhs_controller.Equals(this))
        {
            has_lhs = lhs_controller.TryConnectionAt(connections[0]);
        }
        if (BoardManager.Instance.TryGetTypeAt(connections[1], out rhs_controller) && !rhs_controller.Equals(this))
        {
            has_rhs = rhs_controller.TryConnectionAt(connections[1]);
        }

        if (has_lhs)
        {
            // lhs_controller.UpdateFlowAndVisual TODO
        }
        if (has_rhs)
        {
            // rhs_controller.UpdateFlowAndVisual TODO
        }

        return (has_lhs, has_rhs);
    }

    #region Connector methods
    private void Connect(INewFlowable lhs, INewFlowable rhs)
    {
        // first, get the direction of flow
        var lhs_inout = lhs.GetInOutConfig();
        var rhs_inout = rhs.GetInOutConfig();

        bool ambiguous = lhs_inout.can_output && rhs_inout.can_output && lhs_inout.can_input && rhs_inout.can_input;

        // since pipes take on the configurations of their relationships, the only case where a connection can truly
        // be ambiguous is when two floating pipes are connected by a pipe. Two refineries connected by a pipe would be
        // caught by a different error (flowtype).
        if (ambiguous)
        {
            m_tr.AddTentative(lhs, Relation.Ambiguous);
            m_tr.AddTentative(rhs, Relation.Ambiguous);
        }

        // left is output side - right is input side
        bool left_right = !ambiguous && lhs_inout.can_output && rhs_inout.can_input;
        bool right_left = !ambiguous && lhs_inout.can_input && rhs_inout.can_output;

        if (!left_right && !right_left)
        {
            // ERROR! Impossible connection!
            Debug.LogError("Impossible connection!");
            return;
        }

        var lhs_flow = lhs.GetFlowConfig();
        var rhs_flow = rhs.GetFlowConfig();

        if (left_right)
        {
            Utilities.GetCardinalEstimatePipeflowDirection(m_allPipes[m_lhsIndex], m_lhsConnectionPos, out m_lhsFlowDir);
            Utilities.GetCardinalEstimatePipeflowDirection(m_rhsConnectionPos, m_allPipes[m_rhsIndex], out m_rhsFlowDir);

            if (lhs_flow.out_type != rhs_flow.in_type
                && lhs_flow.out_type != FlowType.Ambiguous
                && rhs_flow.in_type != FlowType.Ambiguous)
            {
                // ERROR! Invalid flow!
                Debug.LogError("Invalid flow!");
                return;
            }

            m_tr.AddTentative(lhs, Relation.Child);
            m_tr.AddTentative(rhs, Relation.Parent);
        }

        if (right_left)
        {
            Utilities.GetCardinalEstimatePipeflowDirection(m_allPipes[m_rhsIndex], m_rhsConnectionPos, out m_rhsFlowDir);
            Utilities.GetCardinalEstimatePipeflowDirection(m_lhsConnectionPos, m_allPipes[m_lhsIndex], out m_lhsFlowDir);

            if (lhs_flow.in_type != rhs_flow.out_type
                && lhs_flow.in_type != FlowType.Ambiguous 
                && rhs_flow.out_type != FlowType.Ambiguous)
            {
                // ERROR! Invalid flow!
                Debug.LogError("Invalid flow!");
                return;
            }

            m_tr.AddTentative(rhs, Relation.Child);
            m_tr.AddTentative(lhs, Relation.Parent);
        }
    }

    private void ConnectSingleSide(INewFlowable side)
    {
        var in_out = side.GetInOutConfig();

        if (in_out.can_output && in_out.can_input)
        {
            // we can't do anything with this.
            m_tr.AddTentative(side, Relation.Ambiguous);
        }
        else if (in_out.can_output)
        {
            // if just output, we must be an output.
            Utilities.GetCardinalEstimatePipeflowDirection(m_allPipes[m_lhsIndex], m_lhsConnectionPos, out m_lhsFlowDir);
            Utilities.GetCardinalEstimatePipeflowDirection(m_rhsConnectionPos, m_allPipes[m_rhsIndex], out m_rhsFlowDir);

            m_tr.AddTentative(side, Relation.Parent);
        }
        else if (in_out.can_input)
        {
            // if just input, we must be an input
            Utilities.GetCardinalEstimatePipeflowDirection(m_allPipes[m_rhsIndex], m_rhsConnectionPos, out m_rhsFlowDir);
            Utilities.GetCardinalEstimatePipeflowDirection(m_lhsConnectionPos, m_allPipes[m_lhsIndex], out m_lhsFlowDir);

            m_tr.AddTentative(side, Relation.Child);
        }
    }
    #endregion

    public void OnTick()
    {
        // TODO
    }

    public float SendFlow()
    {
        var child = m_tr.GetChildren()[0];

        return (child != null) ? child.SendFlow() : 0f;
    }

    public TreeRelationship GetTreeRelationship() => m_tr;

    // there are no hard/soft connections bc of how pipes get rid of places that already have controllers in them.
    // a "hard" connection only exists in semantics.
    public IReadOnlyList<Vector2Int> GetConnectionPositions() => new List<Vector2Int> { m_lhsConnectionPos, m_rhsConnectionPos };

    private bool TryConnectionAt(Vector2Int position)
    {
        bool is_lhs = position.Equals(m_allPipes[m_lhsIndex]);
        bool is_rhs = position.Equals(m_allPipes[m_rhsIndex]);

        if (!is_lhs && !is_rhs) return false;
        else if (is_lhs) return IsPositionFree(m_lhsConnectionPos);
        else if (is_rhs) return IsPositionFree(m_rhsConnectionPos);
        else throw new System.ArgumentException("Position was not connected to a pipe endpoint!");
    }

    public void UpdateFlowAndVisual(Vector2Int a, Vector2Int b, bool _)
    {
        // TODO
    }

    private bool IsPositionFree(Vector2Int position)
    {
        if (!BoardManager.Instance.IsTileOccupied(position)) return true;

        if (!BoardManager.Instance.TryGetTypeAt<INewFlowable>(position, out var component)) return true;

        return !m_tr.IsInRelationshipWith(component);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // clear all relevant pipe tiles from supermap
        int endex = Mathf.Min(m_allPipes.Count - 1, m_rhsIndex + 1);
        for (int i = m_lhsIndex; i < endex; i++)
        {
            BoardManager.Instance.ClearSupermapTile(m_allPipes[i]);
        }
    }

    // TODO make flow direction and flowconfig/inoutconfig reset when relationships are changed (not just when created)

    #region Pipe connection? helper methods
    public bool DoesPipeSystemReceiveInputFromTile(Vector2Int tile_pos)
    {
        if (tile_pos.Equals(m_lhsConnectionPos) && m_lhsFlowDir != PipeFlowDirection.Invalid)
        {
            return (m_allPipes[m_lhsIndex] - Utilities.GetPipeFlowDirOffset(m_lhsFlowDir)).Equals(tile_pos);
        }
        else if (tile_pos.Equals(m_rhsConnectionPos) && m_rhsFlowDir != PipeFlowDirection.Invalid)
        {
            return (m_allPipes[m_rhsIndex] - Utilities.GetPipeFlowDirOffset(m_rhsFlowDir)).Equals(tile_pos);
        }

        return false;
    }

    public bool DoesPipeSystemOutputToTile(Vector2Int tile_pos)
    {
        if (tile_pos.Equals(m_lhsConnectionPos) && m_lhsFlowDir != PipeFlowDirection.Invalid)
        {
            return (m_allPipes[m_lhsIndex] + Utilities.GetPipeFlowDirOffset(m_lhsFlowDir)).Equals(tile_pos);
        }
        else if (tile_pos.Equals(m_rhsConnectionPos) && m_rhsFlowDir != PipeFlowDirection.Invalid)
        {
            return (m_allPipes[m_rhsIndex] + Utilities.GetPipeFlowDirOffset(m_rhsFlowDir)).Equals(tile_pos);
        }

        return false;
    }
    #endregion

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        var offset = new Vector3(0.5f, 0.5f);

        var s_dir = Utilities.GetPipeFlowDirOffset(m_lhsFlowDir);
        var e_dir = Utilities.GetPipeFlowDirOffset(m_rhsFlowDir);

        var s_rot = Quaternion.Euler(-Vector2.SignedAngle(Vector2.up, s_dir) + 90, 90f, 90f);
        var e_rot = Quaternion.Euler(-Vector2.SignedAngle(Vector2.up, e_dir) + 90, 90f, 90f);

        var s_pos = Utilities.Vector2IntToVector3(m_allPipes[m_lhsIndex]) + offset - Utilities.Vector2IntToVector3(Utilities.GetPipeFlowDirOffset(m_lhsFlowDir)) * .35f;
        var e_pos = Utilities.Vector2IntToVector3(m_allPipes[m_rhsIndex]) + offset + Utilities.Vector2IntToVector3(Utilities.GetPipeFlowDirOffset(m_rhsFlowDir)) * .35f;

        if (m_lhsFlowDir != PipeFlowDirection.Invalid)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(m_debugMesh, 0, s_pos, s_rot, new Vector3(0.2f, 0.1f, .2f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(s_pos, Vector3.one * 0.2f);
        }

        if (m_rhsFlowDir != PipeFlowDirection.Invalid)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireMesh(m_debugMesh, 0, e_pos, e_rot, new Vector3(0.2f, 0.1f, .2f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(e_pos, Vector3.one * 0.2f);
        }
    }
} 
