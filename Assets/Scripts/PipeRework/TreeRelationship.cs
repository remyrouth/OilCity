using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeRelationship
{
    private IList<INewFlowable> m_children;
    private IList<INewFlowable> m_parents;
    private IList<INewFlowable> m_tentative;

    private int m_maxChildren;
    private int m_maxParents;

    public TreeRelationship(int max_children, int max_parents)
    {
        m_maxChildren = max_children;
        m_maxParents = max_parents;

        m_children = new List<INewFlowable>();
        m_parents = new List<INewFlowable>();
        m_tentative = new List<INewFlowable>();
    }

    public void AddTentative(INewFlowable tentative)
    {
        if (m_tentative.Contains(tentative)) throw new ArgumentException("Duplicate tenative relationship added!");

        m_tentative.Add(tentative);
    }

    public void MoveToParent(INewFlowable parent)
    {
        if (m_parents.Contains(parent)) throw new ArgumentException("Duplicate parent relationship added!");
        if (m_parents.Count > m_maxParents) throw new ArgumentException("Max number of parent relationships exceeded!");

        m_parents.Add(parent);
        m_tentative.Remove(parent);
    }

    public void MoveToChild(INewFlowable child)
    {
        if (m_children.Contains(child)) throw new ArgumentException("Duplicate child relationship added!");
        if (m_children.Count > m_maxChildren) throw new ArgumentException("Max number of child relationships exceeded!");

        m_children.Add(child);
        m_tentative.Remove(child);
    }

    public IList<INewFlowable> GetChildren() => m_children;
    public IList<INewFlowable> GetParents() => m_parents;


    public void RemoveParent(INewFlowable parent) => m_parents.Remove(parent);

    public void RemoveChild(INewFlowable child) => m_children.Remove(child);


    public bool HasMaxParents() => m_parents.Count >= m_maxParents;

    public bool HasMaxChildren() => m_children.Count >= m_maxChildren;

}
