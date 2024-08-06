using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RelationCollection : IEqualityComparer<(Game.New.IFlowable flowable, Relation relation)>, IEnumerable<(Game.New.IFlowable flowable, Relation relation)>
{
    private HashSet<(Game.New.IFlowable flowable, Relation relation)> m_relations;

    public RelationCollection()
    {
        m_relations = new HashSet<(Game.New.IFlowable flowable, Relation relation)>();
    }

    public void SetRelation(Game.New.IFlowable flowable, Relation relation)
    {
        m_relations.Add((flowable, relation));
    }

    public void ClearRelation(Game.New.IFlowable flowable)
    {
        m_relations.RemoveWhere(i => i.flowable.Equals(flowable));
    }

    public Relation GetFlowableRelation(Game.New.IFlowable flowable)
    {
        return m_relations
            .Where(i => i.flowable.Equals(flowable))
            .First()
            .relation;
    }

    public List<(Game.New.IFlowable flowable, Relation relation)> GetRelationFlowables(Relation r)
    {
        return m_relations.Where(p => p.relation.Equals(r)).ToList();
    }

    #region interface methods
    public bool Equals((Game.New.IFlowable flowable, Relation relation) x, (Game.New.IFlowable flowable, Relation relation) y)
    {
        return x.flowable.Equals(y.flowable);
    }

    public int GetHashCode((Game.New.IFlowable flowable, Relation relation) obj)
    {
        return obj.flowable.GetHashCode();
    }

    public IEnumerator<(Game.New.IFlowable flowable, Relation relation)> GetEnumerator()
    {
        return m_relations.GetEnumerator();
    }

    // dunno what this is for
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)m_relations).GetEnumerator();
    }
    #endregion
}
