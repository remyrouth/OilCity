using System;
using System.Collections.Generic;
using UnityEngine;

public class OilWellController : PayrateBuildingController, IFlowable
{
    public const float BASE_OIL_RATE = 0.01f;
    private const float OIL_RATE_DELTA = 0.002f;

    private IFlowable m_output;
    private List<IFlowable> m_inputs;
    [SerializeField] private ParticleSystem _spilloutEffect;
    private int _tickTimer;
    private int PaymentTimer => 5;
    public event Action<float> OnOilMined;
    private void Start()
    {
        //OnOilMined += IndicateOilAmountMined;
    }
    private void IndicateOilAmountMined(float oilMined)
    {
        PopupValuesPool.Instance.GetFromPool<SimpleTextPopup>(PopupValuesPool.PopupValueType.OilMade)
            .Initialize(((int)(oilMined * 10000)).ToString(), ActionsPivot + Vector2.right);
    }
    public (FlowType type, float amount) SendFlow()
    {
        float amountMined = 0;
        float flowRate = 0f;
        _tickTimer++;
        if (_tickTimer == PaymentTimer)
        {
            _tickTimer = 0;
            PayWorkers();
        }

        flowRate = CurrentMineRate() / (config.size.x * config.size.y);


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
    public float CurrentMineRate()
    {
        switch (CurrentPaymentMode)
        {
            case PaymentMode.LOW:
                return (BASE_OIL_RATE - OIL_RATE_DELTA) / (config.size.x * config.size.y);
            case PaymentMode.MEDIUM:
                return BASE_OIL_RATE / (config.size.x * config.size.y);
            case PaymentMode.HIGH:
                return (BASE_OIL_RATE + OIL_RATE_DELTA) / (config.size.x * config.size.y);
            default:
                return BASE_OIL_RATE;
        }
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
                bool valid = pipe.WouldFlowContentsBeValid(this, peripheral_to);
                if (pipe.DoesPipeSystemReceiveInputFromTile(peripheral_to) && valid)
                {
                    if (m_output != null)
                    {
                        // more than one output pipe discovered
                        // ping the pipe? display a notif that this pipe isnt going to be used?
                        pipe.ToggleSystem(peripheral_to, true);
                        pipe.MarkSystemInvalid(peripheral_to);
                        QuickNotifManager.Instance.PingSpot(QuickNotifManager.PingType.NoConnection, Utilities.Vector2IntToVector3(peripheral_to));

                        return;
                    }

                    SetParent(pipe);
                    pipe.ToggleSystem(peripheral_to, true);
                    QuickNotifManager.Instance.PingSpot(QuickNotifManager.PingType.Connection, Utilities.Vector2IntToVector3(peripheral_to));
                }
                else if (pipe.DoesPipeSystemOutputToTile(peripheral_to) && valid)
                {
                    pipe.ToggleSystem(peripheral_to, false);
                    pipe.MarkSystemInvalid(peripheral_to);
                    QuickNotifManager.Instance.PingSpot(QuickNotifManager.PingType.NoConnection, Utilities.Vector2IntToVector3(peripheral_to));
                    // ping the pipe? display a notif that wells cant have inputs?
                }
                else
                {
                    pipe.ToggleSystem(peripheral_to, false);
                    QuickNotifManager.Instance.PingSpot(QuickNotifManager.PingType.NoConnection, Utilities.Vector2IntToVector3(peripheral_to));
                }
            }
        }
    }

    /// <summary>
    /// An oil well can only handle output pipes.
    /// </summary>
    /// <returns></returns>
    public (bool can_input, bool can_output) GetInOutConfig() => (false, m_output == null);

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
        var flow = SendFlow();

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
    }

    protected override void DecreaseProductivity()
    {
    }

    public (FlowType in_type, FlowType out_type) GetFlowConfig()
    {
        return (FlowType.None, FlowType.Oil);
    }
}
