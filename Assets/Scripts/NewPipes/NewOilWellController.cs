using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewOilWellController : BuildingController<BuildingScriptableObject>, Game.New.IFlowable
{
    private RelationCollection m_relations = new();

    protected override void CreateInitialConnections(Vector2Int pos)
    {
        var pipes = BoardManager.Instance.GetPeripheralTileObjectsForBuilding(pos, config.size);

        foreach (var (_, tile) in pipes)
        {
            if (!tile.TryGetComponent<IConnection>(out var connection)) continue;

            bool valid_o = connection.IsValidConnection(this, Relation.Output);
            bool valid_i = connection.IsValidConnection(this, Relation.Input);

            if (valid_o && valid_i) connection.EstablishConnection(this, Relation.Ambiguous);
            else if (valid_o) connection.EstablishConnection(this, Relation.Output);
            else if (valid_i) connection.EstablishConnection(this, Relation.Input);
            else connection.EstablishConnection(this, Relation.Invalid);
        }
    }

    public void EstablishConnection(Game.New.IFlowable f, Relation r)
    {
        SetRelation(f, r);
        f.SetRelation(this, NewPipeHelper.Flip(r));
    }

    public void EvaluateConnections()
    {
        // pass
    }

    public bool IsValidConnection(Game.New.IFlowable c, Relation r)
    {
        var ft = c.GetFlowConfig();

        if (r == Relation.Output && m_relations.GetRelationFlowables(Relation.Output).Count > 0) return false; // we can only have one output at a time
        if (r == Relation.Input || r == Relation.Invalid) return false;
        if (ft.input == FlowType.Kerosene || ft.input == FlowType.None) return false;

        return true;
    }

    public void Remove()
    {
        foreach (var r in m_relations)
        {
            r.flowable.ClearRelation(this);
            r.flowable.EvaluateConnections();
        }
    }


    public void ClearRelation(Game.New.IFlowable f) => m_relations.ClearRelation(f);
    public void SetRelation(Game.New.IFlowable f, Relation r) => m_relations.SetRelation(f, r);
    public (FlowType input, FlowType output) GetFlowConfig() => (FlowType.Oil, FlowType.Kerosene);
    public (bool can_input, bool can_output) GetIOConfig() => (true, true);
    public RelationCollection GetRelations() => m_relations;
    public void OnTick()
    {
        Debug.Log(gameObject + " is in the forest!");
    }
}
