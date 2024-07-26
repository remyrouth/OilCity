using System.Collections.Generic;
using UnityEngine;

public sealed class OilWellController : PayrateBuildingController, IFlowable
{
    private const float BASE_OIL_RATE = 0.01f;

    private IFlowable m_output;
    private List<IFlowable> m_inputs;
    [SerializeField] private GameObject _spilloutEffect;
    public (FlowType type, float amount) SendFlow()
    {
        float amountMined = 0;
        float flowRate = BASE_OIL_RATE / (config.size.x * config.size.y);
        for (int x = 0; x < config.size.x; x++)
        {
            for (int y = 0; y < config.size.y; y++)
            {
                float oilAvailable = BoardManager.Instance.OilEvaluator.GetValueAtPosition(Anchor.x + x, Anchor.y + y);
                float minedFromTile = Mathf.Clamp(oilAvailable, 0, flowRate);
                amountMined += minedFromTile;
                BoardManager.Instance.OilEvaluator.IncreaseAmountMinedAtPosition(Anchor.x + x, Anchor.y + y, minedFromTile);
            }
        }
        return (FlowType.Oil, amountMined);
    }

    void Awake()
    {
        m_inputs = new List<IFlowable>();
        _spilloutEffect.SetActive(false);
    }

    protected override void CreateInitialConnections(Vector2Int with_position)
    {
        m_output = null;
        m_inputs.Clear();

        var peripherals = BoardManager.Instance.GetPeripheralTileObjectsForBuilding(with_position, config.size);

        foreach (var p in peripherals)
        {
            if (p.TryGetComponent<PipeController>(out var pipe))
            {
                if (pipe.DoesPipeSystemReceiveInputFromTile(with_position))
                {
                    if (m_output != null)
                    {
                        // more than one output pipe discovered
                        // ping the pipe? display a notif that this pipe isnt going to be used?
                        // TODO
                    }

                    m_output = pipe;
                }
                else if (pipe.DoesPipeSystemOutputToTile(with_position))
                {
                    // ping the pipe? display a notif that wells cant have inputs?
                }
            }
        }
    }

    /// <summary>
    /// An oil well can only handle output pipes.
    /// </summary>
    /// <returns></returns>
    public (bool can_input, bool can_output) GetFlowConfig() => (false, true);

    #region Tree stuff
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
        _spilloutEffect.SetActive(false);
    }
    #endregion

    public void OnTick()
    {
        var flow = SendFlow();
        _spilloutEffect.SetActive(flow.amount>0);
        if (flow.amount == 0)
            return;
        Debug.LogWarning("Oil well has overflowed " + flow);
    }
}
