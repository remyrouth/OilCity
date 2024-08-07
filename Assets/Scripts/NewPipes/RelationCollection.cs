using Game.New;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RelationCollection : IEqualityComparer<(Game.New.IFlowable flowable, Relation relation)>, IEnumerable<(Game.New.IFlowable flowable, Relation relation)>
{
    private Game.New.IFlowable m_self;
    private HashSet<(Game.New.IFlowable flowable, Relation relation)> m_relations;

    public RelationCollection(Game.New.IFlowable self)
    {
        m_self = self;
        m_relations = new HashSet<(Game.New.IFlowable flowable, Relation relation)>();
    }

    public void SetRelation(Game.New.IFlowable flowable, Relation relation)
    {
        bool exists = m_relations.Where(i => i.flowable.Equals(flowable)).Count() > 0;
        ClearRelation(flowable, false);

        m_relations.Add((flowable, relation));

        UpdateForestStatus();
    }

    public void ClearRelation(Game.New.IFlowable flowable, bool do_check = true)
    {
        m_relations.RemoveWhere(i => i.flowable.Equals(flowable));

        if (do_check) UpdateForestStatus();
    }

    public void UpdateForestStatus()
    {
        bool has_parent = GetRelationFlowables(Relation.Output).Count > 0;

        bool in_forest = TimeManager.Instance.m_tickableForest.Contains(m_self);

        if (in_forest && has_parent)
        {
            TimeManager.Instance.m_tickableForest.Remove(m_self);
        }
        else if (!in_forest && !has_parent)
        {
            TimeManager.Instance.m_tickableForest.Add(m_self);
        }
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
