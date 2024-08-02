using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class OilWellController : PayrateBuildingController, IFlowable
{
    public const float BASE_OIL_RATE = 0.01f;
    private const float OIL_RATE_DELTA = 0.002f;

    private IFlowable m_output;
    private List<IFlowable> m_inputs;
    [SerializeField] private ParticleSystem _spilloutEffect;
    private int _tickTimer;
    private int PaymentTimer => 5;
    public event Action<float> OnOilMined;
    public (FlowType type, float amount) SendFlow()
    {
        float amountMined = 0;
        float flowRate = 0f;
        switch (CurrentPaymentMode)
        {
            case PaymentMode.LOW:
                flowRate = (BASE_OIL_RATE - OIL_RATE_DELTA) / (config.size.x * config.size.y);
                break;
            case PaymentMode.MEDIUM:
                flowRate = BASE_OIL_RATE / (config.size.x * config.size.y);
                break;
            case PaymentMode.HIGH:
                flowRate = (BASE_OIL_RATE + OIL_RATE_DELTA) / (config.size.x * config.size.y);
                break;
        }
        
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
        OnOilMined?.Invoke(amountMined);
        return (FlowType.Oil, amountMined);
    }

    void Awake()
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
                    // ping the pipe? display a notif that wells cant have inputs?
                }
            }
        }
    }

    /// <summary>
    /// An oil well can only handle output pipes.
    /// </summary>
    /// <returns></returns>
    public (bool can_input, bool can_output) GetInOutConfig() => (false, true);

    #region Tree stuff
    public void AddChild(IFlowable child)
    {
        throw new System.InvalidOperationException();
    }

    public void DisownChild(IFlowable child)
    {
        throw new System.InvalidOperationException();
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
        _spilloutEffect.Stop();
    }
    #endregion

    public void OnTick()
    {
        _tickTimer++;
        var flow = SendFlow();
        if (_tickTimer == PaymentTimer)
        {
            _tickTimer = 0;
            PayWorkers();
        }
        if (flow.amount > 0)
            _spilloutEffect.Play();
        else
            _spilloutEffect.Stop();
        if (flow.amount == 0)
            return;
        Debug.LogWarning("Oil well has overflowed " + flow);
    }

    protected override void IncreaseProductivity()
    {
        throw new System.NotImplementedException();
    }

    protected override void DecreaseProductivity()
    {
        throw new System.NotImplementedException();
    }

    public (FlowType in_type, FlowType out_type) GetFlowConfig()
    {
        return (FlowType.None, FlowType.Oil);
    }
}
