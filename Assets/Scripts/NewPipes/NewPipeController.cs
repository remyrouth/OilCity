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

    public void InitializePipe(Vector2Int lhs, Vector2Int rhs, PipeFlowDirection lhs_pipe_dir, PipeFlowDirection rhs_pipe_dir, List<Vector2Int> pipes)
    {
        int start_i = -1, end_i = -1;
        for (int i = 0; i < pipes.Count; i++)
        {
            if (pipes[i].Equals(lhs)) start_i = i;
            if (pipes[i].Equals(rhs)) end_i = i;
        }

        if (start_i == -1 || end_i == -1) throw new System.ArgumentException("start or end positions were not found in pipe point list!");

        m_pipes = pipes.GetRange(start_i, end_i - start_i + 1);

        m_lhsConnectionPos = m_pipes[0] - Utilities.GetPipeFlowDirOffset(lhs_pipe_dir);
        m_rhsConnectionPos = m_pipes[^1] + Utilities.GetPipeFlowDirOffset(rhs_pipe_dir);
    }

    public void SetTileActions(List<TileAction> actions)
    {
        this.TileActions = actions;
    }
    protected override void CreateInitialConnections(Vector2Int _)
    {
        var (lhs, rhs) = GetFlowablesAtEndpoints();

        bool no_lhs = lhs == null;
        bool no_rhs = rhs == null;

        // if a side doesn't have a connection, check/establish one with the side that does.
        if (no_lhs && no_rhs) return;
        else if (no_lhs)
        {
            bool valid_o = rhs.IsValidConnection(this, Relation.Output);
            bool valid_i = rhs.IsValidConnection(this, Relation.Input);

            Relation to_assign = Relation.Invalid;
            if (valid_o && valid_i) to_assign = Relation.Ambiguous;
            else if (valid_o) to_assign = Relation.Output;
            else if (valid_i) to_assign = Relation.Input;

            rhs.EstablishConnection(this, to_assign);

            return;
        }
        else if (no_rhs)
        {
            bool valid_o = lhs.IsValidConnection(this, Relation.Output);
            bool valid_i = lhs.IsValidConnection(this, Relation.Input);

            Relation to_assign = Relation.Invalid;
            if (valid_o && valid_i) to_assign = Relation.Ambiguous;
            else if (valid_o) to_assign = Relation.Output;
            else if (valid_i) to_assign = Relation.Input;

            lhs.EstablishConnection(this, to_assign);

            return;
        }

        // otherwise, establish them with each other.
        bool valid_l_r = lhs.IsValidConnection(rhs, Relation.Output);
        bool valid_r_l = rhs.IsValidConnection(lhs, Relation.Output);

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
            lhs.EstablishConnection(this, Relation.Output);
            rhs.EstablishConnection(this, Relation.Input);
        }
        else
        {
            lhs.EstablishConnection(this, Relation.Input);
            rhs.EstablishConnection(this, Relation.Output);
        }
    }

    public void EvaluateConnections()
    {
        foreach (var side in m_relations)
        {
            side.flowable.EstablishConnection(this, Relation.Ambiguous);
        }
    }

    public bool IsValidConnection(Game.New.IFlowable c, Relation r)
    {
        var endpoint = GetOtherConnection(c);

        if (endpoint != null && endpoint.IsValidConnection(c, r)) return true;
        return false;
    }

    public void Remove()
    {
        foreach (var r in m_relations)
        {
            r.flowable.ClearRelation(this);
            r.flowable.EvaluateConnections();
        }
    }

    public void EstablishConnection(Game.New.IFlowable f, Relation r)
    {
        SetRelation(f, r);
        f.SetRelation(this, NewPipeHelper.Flip(r));

        var other = GetOtherConnection(f);
        if (other == null) Debug.LogWarning("other connection with " + gameObject + " is null.");
        other?.EstablishConnection(this, r);
    }

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

        var input = i_flow.Count > 0 ? i_flow.First().flowable.GetFlowConfig().input : FlowType.Ambiguous; // TODO should this be None?
        var output = o_flow.Count > 0 ? o_flow.First().flowable.GetFlowConfig().output : FlowType.Ambiguous;

        return (input, output);
    }

    private IConnection GetOtherConnection(IConnection c)
    {
        var (lhs_c, rhs_c) = GetFlowablesAtEndpoints();

        bool lhs_taken = lhs_c != null;
        bool rhs_taken = rhs_c != null;

        if (!lhs_taken && BoardManager.Instance.IsTileOccupiedByInProgressBuild(m_lhsConnectionPos)) return rhs_c;
        if (!rhs_taken && BoardManager.Instance.IsTileOccupiedByInProgressBuild(m_rhsConnectionPos)) return lhs_c;

        if (lhs_taken && lhs_c.Equals(c)) return rhs_c;
        if (rhs_taken && rhs_c.Equals(c)) return lhs_c;
        return null; // must be an invalid connection
        
        // TODO BAD: this is needed bc buildings in the process of being placed dont occupy the map. Note that this might also break pipe-on-pipe connections at non-endpoints!

    }

    private (Game.New.IFlowable lhs, Game.New.IFlowable rhs) GetFlowablesAtEndpoints()
    {
        BoardManager.Instance.TryGetTypeAt<Game.New.IFlowable>(m_lhsConnectionPos, out var lhs_c);
        BoardManager.Instance.TryGetTypeAt<Game.New.IFlowable>(m_rhsConnectionPos, out var rhs_c);

        return (lhs_c, rhs_c);
    }


    public void ClearRelation(Game.New.IFlowable f) => m_relations.ClearRelation(f);

    public void SetRelation(Game.New.IFlowable f, Relation r) => m_relations.SetRelation(f, r);

    public RelationCollection GetRelations() => m_relations;

    public void OnTick()
    {
        Debug.Log(gameObject + " is in the forest!");
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
}
