using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeRelationship
{
    private IList<INewFlowable> m_children;
    private IList<INewFlowable> m_parents;
    private IList<(INewFlowable flowable, Relation type)> m_tentative;

    private int m_maxChildren;
    private int m_maxParents;

    public TreeRelationship(int max_children, int max_parents)
    {
        m_maxChildren = max_children;
        m_maxParents = max_parents;

        m_children = new List<INewFlowable>();
        m_parents = new List<INewFlowable>();
        m_tentative = new List<(INewFlowable flowable, Relation type)>();
    }

    public void AddTentative(INewFlowable tentative, Relation type)
    {
        if (TentativeContains(tentative)) throw new ArgumentException("Duplicate tenative relationship added!");

        m_tentative.Add((tentative, type));
    }

    public void DirectAddParent(INewFlowable parent)
    {
        if (m_parents.Count > m_maxParents) throw new ArgumentException("Max number of parent relationships exceeded!");
        m_parents.Add(parent);
    }

    public void DirectAddChild(INewFlowable child)
    {
        if (m_children.Count > m_maxChildren) throw new ArgumentException("Max number of child relationships exceeded!");
        m_children.Add(child);
    }

    public void TentativeToParent(INewFlowable parent)
    {
        if (!TentativeContains(parent)) throw new ArgumentException("Given tentative wasn't registered!");
        if (m_parents.Contains(parent)) throw new ArgumentException("Duplicate parent relationship added!");
        
        DirectAddParent(parent);

        for (int i = 0; i < m_tentative.Count; i++)
        {
            if (m_tentative[i].flowable.Equals(parent))
            {
                m_tentative.RemoveAt(i);
                return;
            }
        }
    }

    public void MoveToChild(INewFlowable child)
    {
        if (!TentativeContains(child)) throw new ArgumentException("Given tentative wasn't registered!");
        if (m_children.Contains(child)) throw new ArgumentException("Duplicate child relationship added!");

        DirectAddChild(child);

        for (int i = 0; i < m_tentative.Count; i++)
        {
            if (m_tentative[i].flowable.Equals(child))
            {
                m_tentative.RemoveAt(i);
                return;
            }
        }
    }

    public IList<INewFlowable> GetChildren() => m_children;
    public IList<INewFlowable> GetParents() => m_parents;
    public IList<(INewFlowable flowable, Relation type)> GetTentative(Relation type)
        => m_tentative.Where(a => a.type == type).ToList();


    public void RemoveParent(INewFlowable parent) => m_parents.Remove(parent);

    public void RemoveChild(INewFlowable child) => m_children.Remove(child);


    public bool HasMaxParents() => m_parents.Count >= m_maxParents;

    public bool HasMaxChildren() => m_children.Count >= m_maxChildren;

    public bool IsInRelationshipWith(INewFlowable item) 
        => m_children.Contains(item) || m_parents.Contains(item) || TentativeContains(item);

    private bool TentativeContains(INewFlowable item) => m_tentative.Count(a => a.flowable.Equals(item)) > 0;

}
