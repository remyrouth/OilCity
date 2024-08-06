using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewPipeController : BuildingController<BuildingScriptableObject>, Game.New.IFlowable
{
    private RelationCollection m_relations = new();

    private List<Vector2Int> m_pipes;

    private Vector2Int m_lhsConnectionPos;
    private Vector2Int m_rhsConnectionPos;

    public void InitializePipes(Vector2Int lhs, Vector2Int rhs, List<Vector2Int> pipes)
    {
        // todo
    }

    protected override void CreateInitialConnections(Vector2Int _)
    {
        var (lhs, rhs) = GetFlowablesAtEndpoints();

        bool valid_l_r = lhs.IsValidConnection(rhs.GetIOConfig(), rhs.GetFlowConfig(), Relation.Output);
        bool valid_r_l = rhs.IsValidConnection(lhs.GetIOConfig(), lhs.GetFlowConfig(), Relation.Output);

        if (!valid_l_r && !valid_r_l )
        {
            lhs.EstablishConnection(this, Relation.Invalid);
            rhs.EstablishConnection(this, Relation.Invalid);
        }
        else if (valid_l_r && valid_r_l)
        {
            lhs.EstablishConnection(this, Relation.Ambiguous);
            rhs.EstablishConnection(this, Relation.Ambiguous);
        }
        else if (valid_l_r)
        {
            lhs.EstablishConnection(this, Relation.Input);
            rhs.EstablishConnection(this, Relation.Output);
        }
        else
        {
            lhs.EstablishConnection(this, Relation.Output);
            rhs.EstablishConnection(this, Relation.Input);
        }
    }

    public void EvaluateConnections()
    {
        foreach (var side in m_relations)
        {
            side.flowable.EstablishConnection(this, Relation.Ambiguous);
        }
    }

    public bool IsValidConnection((bool can_input, bool can_output) io, (FlowType in_type, FlowType out_type) fc, Relation r)
    {
        throw new System.NotImplementedException();
    }

    public void Remove()
    {
        foreach (var r in m_relations)
        {
            r.flowable.ClearRelation(this);
        }
    }

    public void EstablishConnection(Game.New.IFlowable f, Relation r)
    {
        SetRelation(f, r);
        f.SetRelation(this, NewPipeHelper.Flip(r));

        var other = GetControllerAtOtherEnd(f);
        other.EstablishConnection(this, r);
    }

    public void ClearRelation(Game.New.IFlowable f) =>  m_relations.ClearRelation(f);

    public void SetRelation(Game.New.IFlowable f, Relation r) => m_relations.SetRelation(f, r);

    public (bool can_input, bool can_output) GetIOConfig()
    {
        var i_flow = m_relations.GetRelationFlowables(Relation.Input);
        var o_flow = m_relations.GetRelationFlowables(Relation.Output);

        bool can_input = i_flow.Count > 0 ? i_flow.First().flowable.GetIOConfig().can_input : true;
        bool can_output = o_flow.Count > 0 ? o_flow.First().flowable.GetIOConfig().can_output : true;

        return (can_input, can_output);
    }

    public (FlowType input, FlowType output) GetFlowConfig()
    {
        var i_flow = m_relations.GetRelationFlowables(Relation.Input);
        var o_flow = m_relations.GetRelationFlowables(Relation.Output);

        var input = i_flow.Count > 0 ? i_flow.First().flowable.GetFlowConfig().input : FlowType.None; // TODO should this be None?
        var output = o_flow.Count > 0 ? o_flow.First().flowable.GetFlowConfig().output : FlowType.None;

        return (input, output);
    }

    private Game.New.IFlowable GetControllerAtOtherEnd(Game.New.IFlowable controller)
    {
        return null; // TODO
    }

    private (Game.New.IFlowable lhs, Game.New.IFlowable rhs) GetFlowablesAtEndpoints()
    {
        return (null, null); // TODO
    }
}
