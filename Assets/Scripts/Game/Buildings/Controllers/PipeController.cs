using System.Collections.Generic;
using System;
using UnityEngine;

public sealed class PipeController : BuildingController<BuildingScriptableObject>, IFlowable
{
    private enum PipeFlowDirection
    {
        North,
        South,
        East,
        West
    }

    private IFlowable m_start; // where you get the flow from
    private IFlowable m_end; // where the flow goes to
    private PipeFlowDirection m_startDirection; // the orientation of the start pipe
    private PipeFlowDirection m_endDirection; // the orientation of the end pipe
    private Vector2Int m_startPipePos; // position of the start pipe
    private Vector2Int m_endPipePos; // position of the end pipe

    // TODO init method for JUST PIPES
    /*
     * will need to set the values of the pipeflow and vector2int properties. needs to be a special constructor.
    */ 

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

    public bool IsPipeConnected(GameObject other)
    {
        if (!other.TryGetComponent<IFlowable>(out var flowable))
        {
            return false;
        }

        // use pipe params to assess if this pipe goes into the input building

        if (!flowable.Equals(m_start) || !flowable.Equals(m_end))
        {
            return false;
        }

        return true;
    }
}
