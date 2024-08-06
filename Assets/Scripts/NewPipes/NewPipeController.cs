using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewPipeController : BuildingController<BuildingScriptableObject>, Game.New.IFlowable
{
    private RelationCollection m_relations = new();

    private List<Vector2Int> m_pipes;


    public void InitializePipes()
    {
        // todo
    }

    protected override void CreateInitialConnections(Vector2Int _)
    {

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
        f.SetRelation(this, )
    }

    public void ClearRelation(Game.New.IFlowable f) =>  m_relations.ClearRelation(f);

    public void SetRelation(Game.New.IFlowable f, Relation r) => m_relations.SetRelation(f, r);
}
