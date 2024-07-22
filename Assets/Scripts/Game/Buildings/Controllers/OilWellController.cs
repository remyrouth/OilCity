using System.Collections.Generic;
using UnityEngine;

public sealed class OilWellController : PayrateBuildingController, IFlowable
{
    public const float BASE_OIL_RATE = 1;
    private IFlowable m_output;
    private List<IFlowable> m_inputs;

    public (FlowType type, float amount) SendFlow() => (FlowType.Oil, BASE_OIL_RATE);

    void Awake()
    {
        m_inputs = new List<IFlowable>();
    }

    public void AddChild(IFlowable child)
    {
        if (!m_inputs.Contains(child))
        {
            m_inputs.Add(child);
        }
    }

    public void DisownChild(IFlowable child)
    {
        if (m_inputs.Contains(child))
        {
            m_inputs.Remove(child);
        }
    }

    public List<IFlowable> GetChildren()
    {
        return m_inputs;
    }

    public IFlowable GetParent()
    {
        return m_output;
    }

    public void SetParent(IFlowable parent)
    {
        m_output = parent;
    }

    public void OnTick()
    {
        Debug.LogWarning("Oil well has overflowed " + SendFlow());
    }
}
