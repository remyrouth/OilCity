using UnityEngine;
using System.Collections.Generic;

public sealed class SiloController : BuildingController<BuildingScriptableObject>, IFlowable
{
    public const float BASE_SILO_CAPACITY = 10;
    public const float BASE_SILO_FLOWRATE = 1;
    public float AmountStored { get; private set; }
    public FlowType TypeStored { get; private set; }

    private IFlowable m_output;
    private List<IFlowable> m_inputs;

    public (FlowType type, float amount) SendFlow()
    {
        float liquidSum = 0;
        foreach (var child in m_inputs)
        {
            var received = child.SendFlow();
            if (received.amount == 0)
                continue;
            if (AmountStored == 0)
                TypeStored = received.type;
            if (received.type != TypeStored)
            {
                Debug.LogWarning("Refinery just received Kerosene!!!", gameObject);
                continue;
            }
            liquidSum += received.amount;
        }
        AmountStored += liquidSum;
        float liquidGiven = Mathf.Clamp(BASE_SILO_FLOWRATE, 0, AmountStored);

        if (AmountStored > BASE_SILO_CAPACITY)
        {
            Debug.LogWarning($"There was a spillage of {AmountStored - BASE_SILO_CAPACITY} {TypeStored}!", gameObject);
            AmountStored = BASE_SILO_CAPACITY;
        }
        AmountStored -= liquidGiven;
        return (TypeStored, liquidGiven);
    }

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
        Debug.LogWarning("Silo has overflowed " + SendFlow());
    }
}
