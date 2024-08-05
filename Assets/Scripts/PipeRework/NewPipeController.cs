using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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

        return (in_type, out_type);
    }

    public (bool can_input, bool can_output) GetInOutConfig() => (true, true);

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
        var positions = GetConnectionPositions();

        NewPipeController lhs = null, rhs = null;
        TryGetPipeConnections(ref lhs, ref rhs);
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
            // lhs_controller.UpdateFlowAndVisual
        }
        if (has_rhs)
        {
            // rhs_controller.UpdateFlowAndVisual
        }

        return (has_lhs, has_rhs);
    }

    public void OnTick()
    {
        // TODO
    }

    public float SendFlow()
    {
        // TODO
        return 0.0f;
    }

    public TreeRelationship GetTreeRelationship() => m_tr;

    // there are no hard/soft connections bc of how pipes get rid of places that already have controllers in them.
    // a "hard" connection only exists in semantics.
    public IReadOnlyList<Vector2Int> GetConnectionPositions() => new List<Vector2Int> { m_lhsConnectionPos, m_rhsConnectionPos };

    public bool TryConnectionAt(Vector2Int position)
    {
        bool is_lhs = position.Equals(m_allPipes[m_lhsIndex]);
        bool is_rhs = position.Equals(m_allPipes[m_rhsIndex]);

        if (!is_lhs && !is_rhs) return false;
        else if (is_lhs) return IsPositionFree(m_lhsConnectionPos);
        else if (is_rhs) return IsPositionFree(m_rhsConnectionPos);
        else throw new System.ArgumentException("Position was not connected to a pipe endpoint!");
    }

    private bool IsPositionFree(Vector2Int position)
    {
        if (!BoardManager.Instance.IsTileOccupied(position)) return true;

        if (!BoardManager.Instance.TryGetTypeAt<INewFlowable>(position, out var component)) return true;

        return !m_tr.IsInRelationshipWith(component);
    }
} 
