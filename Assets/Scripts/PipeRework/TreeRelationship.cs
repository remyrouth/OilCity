using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRelationship
{
    private ICollection<IFlowable> m_children;
    private ICollection<IFlowable> m_parents;
    private ICollection<IFlowable> m_tentative;

    private int m_maxChildren;
    private int m_maxParents;

    public TreeRelationship(int max_children, int max_parents)
    {
        m_maxChildren = max_children;
        m_maxParents = max_parents;

        m_children = new HashSet<IFlowable>();
        m_parents = new HashSet<IFlowable>();
        m_tentative = new HashSet<IFlowable>();
    }

    public void AddTentative(IFlowable tentative)
    {
        if (m_tentative.Contains(tentative)) throw new ArgumentException("Duplicate tenative relationship added!");

        m_tentative.Add(tentative);
    }

    public void MoveToParent(IFlowable parent)
    {
        if (m_parents.Contains(parent)) throw new ArgumentException("Duplicate parent relationship added!");
        if (m_parents.Count > m_maxParents) throw new ArgumentException("Max number of parent relationships exceeded!");

        m_parents.Add(parent);
        m_tentative.Remove(parent);
    }

    public void MoveToChild(IFlowable child)
    {
        if (m_children.Contains(child)) throw new ArgumentException("Duplicate child relationship added!");
        if (m_children.Count > m_maxChildren) throw new ArgumentException("Max number of child relationships exceeded!");

        m_children.Add(child);
        m_tentative.Remove(child);
    }


    public void RemoveParent(IFlowable parent) => m_parents.Remove(parent);

    public void RemoveChild(IFlowable child) => m_children.Remove(child);


    public bool HasMaxParents() => m_parents.Count >= m_maxParents;

    public bool HasMaxChildren() => m_children.Count >= m_maxChildren;

}
