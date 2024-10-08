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

    private void Awake()
    {
        m_inputs = new List<IFlowable>();
    }

    protected override void CreateInitialConnections(Vector2Int with_position)
    {
        m_output = null;
        m_inputs.Clear();

        var peripherals = BoardManager.Instance.GetPeripheralTileObjectsForBuilding(with_position, config.size);

        foreach (var (peripheral_to, tile) in peripherals)
        {
            if (tile.TryGetComponent<PipeController>(out var pipe))
            {
                if (pipe.DoesPipeSystemReceiveInputFromTile(peripheral_to))
                {
                    if (m_output != null)
                    {
                        // more than one output pipe discovered
                        // ping the pipe? display a notif that this pipe isnt going to be used?
                        // TODO

                        return;
                    }

                    pipe.AddChild(this);
                    SetParent(pipe);
                }
                else if (pipe.DoesPipeSystemOutputToTile(peripheral_to))
                {
                    pipe.SetParent(this);
                    AddChild(pipe);
                }
            }
        }
    }

    public (bool can_input, bool can_output) GetInOutConfig() => (true, true);

    public (FlowType type, float amount) SendFlow()
    {
        float liquidSum = 0;
        foreach (var child in GetChildren())
        {
            var received = child.SendFlow();
            if (received.amount == 0)
                continue;
            if (AmountStored == 0)
                TypeStored = received.type;
            if (received.type != TypeStored)
            {
                Debug.LogWarning("Refinery just received different fluids!!!", gameObject);
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

    #region icky tree stuff
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
    #endregion

    public void OnTick()
    {
        // if this is called directly, that means there is no output for this building
        Debug.LogWarning("Silo has overflowed " + SendFlow());
    }

    public (FlowType in_type, FlowType out_type) GetFlowConfig()
    {
        throw new System.NotImplementedException();
    }
}
