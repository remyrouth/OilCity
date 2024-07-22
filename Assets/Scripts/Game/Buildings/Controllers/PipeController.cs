using System.Collections.Generic;
using System;
using UnityEngine;

public sealed class PipeController : BuildingController<BuildingScriptableObject>, IFlowable
{
    private IFlowable m_start; // where you get the flow from
    private IFlowable m_end; // where the flow goes to

    public void AddChild(IFlowable child)
    {
        if (m_start == null || !m_start.Equals(child))
        {
            m_start = child;
        }
    }

    public void DisownChild(IFlowable child)
    {
        if (m_start.Equals(child))
        {
            m_start = null;
        }
    }

    public List<IFlowable> GetChildren()
    {
        var singleton_list = new List<IFlowable>
        {
            m_start
        };

        return singleton_list;
    }

    public IFlowable GetParent()
    {
        return m_end;
    }

    public void SetParent(IFlowable parent)
    {
        m_end = parent;
    }

    public void OnTick()
    {
        Debug.LogWarning("Pipe has overflowed " + SendFlow());
    }

    public (FlowType type, float amount) SendFlow()
    {
        return m_start.SendFlow();
    }
}
