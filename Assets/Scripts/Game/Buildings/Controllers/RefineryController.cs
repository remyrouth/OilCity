using System.Collections.Generic;
using UnityEngine;

public sealed class RefineryController : PayrateBuildingController, IFlowable
{
    public const float BASE_REFINERY_FLOWRATE = 1;

    private IFlowable m_output;
    private List<IFlowable> m_inputs;

    protected override void CreateInitialConnections()
    {
        m_output = null;
        m_inputs.Clear();

        var position = BoardManager.ConvertWorldspaceToGrid(transform.position);
        var peripherals = BoardManager.Instance.GetPeripheralTileObjectsForBuilding(position, config.size);

        foreach (var p in peripherals)
        {
            if (p.TryGetComponent<PipeController>(out var pipe))
            {
                if (pipe.IsInputPipeForTile(position))
                {
                    m_inputs.Add(pipe);
                }
                else if (pipe.IsOutputPipeForTile(position))
                {
                    if (m_output != null)
                    {
                        // more than one output pipe discovered
                        // ping the pipe? display a notif that this pipe isnt going to be used?
                        // TODO
                    }

                    m_output = pipe;
                }
            }
        }
    }

    public (FlowType type, float amount) SendFlow()
    {
        float OilSum = 0;
        foreach (var child in m_inputs)
        {
            var received = child.SendFlow();
            if (received.type == FlowType.Kerosene)
            {
                Debug.LogWarning("Refinery just received Kerosene!!!",gameObject);
                continue;
            }
            OilSum += received.amount;
        }
        if (OilSum > BASE_REFINERY_FLOWRATE)
        {
            float diff = OilSum - BASE_REFINERY_FLOWRATE;
            Debug.LogWarning($"Spilled {diff} amount of Kerosene!", gameObject);
            OilSum = BASE_REFINERY_FLOWRATE;
        }
        return (FlowType.Kerosene, OilSum);
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
        Debug.LogWarning("Refinery has overflowed " + SendFlow());
    }

}
