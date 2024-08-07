using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTrainStationController : BuildingController<BuildingScriptableObject>, Game.New.IFlowable
{
    private RelationCollection m_relations = new();

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

        if (r == Relation.Input || r == Relation.Invalid) return false;
        if (ft.input == FlowType.Oil || ft.input == FlowType.None) return false;

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
