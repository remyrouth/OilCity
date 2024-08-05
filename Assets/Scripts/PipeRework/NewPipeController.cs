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
    }

    public void OnTick()
    {
        // TODO
    }

    public float SendFlow()
    {
        throw new System.NotImplementedException();
    }

    public TreeRelationship GetTreeRelationship() => m_tr;
}
