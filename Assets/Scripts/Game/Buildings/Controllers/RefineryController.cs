using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class RefineryController : PayrateBuildingController, IFlowable
{
    private float baseRefineryFlowrate;
    private float keroseneMultiplier;

    private IFlowable m_output;
    private List<IFlowable> m_inputs;
    [SerializeField] private ParticleSystem _spilloutEffect;
    [SerializeField] private ParticleSystem[] _workingSmokeEffects;
    private int _tickTimer;
    private int PaymentTimer => 5;
    public bool IsWorking { get; private set; } = false;
    public event Action<float> OnKeroseneProduced;
    protected override void CreateInitialConnections(Vector2Int with_position)
    {
        m_output = null;
        m_inputs.Clear();

        var peripherals = BoardManager.Instance.GetPeripheralTileObjectsForBuilding(with_position, config.size);

        foreach (var p in peripherals)
        {
            if (p.tile.TryGetComponent<PipeController>(out var pipe))
            {
                if (pipe.DoesPipeSystemReceiveInputFromTile(p.peripheral_to)) // TODO not with_position, but rather the tile of the building that would be connected to
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
                else if (pipe.DoesPipeSystemOutputToTile(p.peripheral_to))
                {
                    pipe.SetParent(this);
                    AddChild(pipe);
                }
            }
        }
    }

    /// <summary>
    /// Refineries can input and output fluid to pipes.
    /// </summary>
    /// <returns></returns>
    public (bool can_input, bool can_output) GetInOutConfig() => (true, true);

    public (FlowType type, float amount) SendFlow()
    {
        float OilSum = 0;
        keroseneMultiplier = GetKeroseneMultiplier();
        foreach (var child in m_inputs)
        {
            var received = child.SendFlow();
            if (received.type == FlowType.Kerosene)
            {
                Debug.LogWarning("Refinery just received Kerosene!!!", gameObject);
                continue;
            }
            IsWorking = true;
            OilSum += received.amount * keroseneMultiplier;
        }
        IsWorking = OilSum > 0;
        HandleWorkingEffects();
        baseRefineryFlowrate = GetBaseRefineryFlowrate();

        if (OilSum > baseRefineryFlowrate)
        {
            float diff = OilSum - baseRefineryFlowrate;
            Debug.LogWarning($"Spilled {diff} amount of Kerosene!", gameObject);
            OilSum = baseRefineryFlowrate;

            _spilloutEffect.Play();
        }
        else
        {
            _spilloutEffect.Stop();
        }

        OnKeroseneProduced?.Invoke(OilSum);
        return (FlowType.Kerosene, OilSum);
    }

    void Awake()
    {
        m_inputs = new List<IFlowable>();
    }

    private float GetBaseRefineryFlowrate()
    {
        return OilWellController.BASE_OIL_RATE * ((int)CurrentPaymentMode + 1);
    }
    private float GetKeroseneMultiplier()
    {

        return CurrentPaymentMode switch
        {
            PaymentMode.LOW => 0.5f,
            PaymentMode.MEDIUM => 1,
            PaymentMode.HIGH => 1.5f,
            _ => 0,
        };

    }

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
        Debug.LogWarning("Refinery has overflowed " + flow);
    }

    protected override void IncreaseProductivity()
    {
        throw new System.NotImplementedException();
    }

    protected override void DecreaseProductivity()
    {
        throw new System.NotImplementedException();
    }
    private void HandleWorkingEffects()
    {
        foreach (var vfx in _workingSmokeEffects)
        {
            if (IsWorking)
                vfx.Play();
            else
                vfx.Stop();
        }
    }

    public (FlowType in_type, FlowType out_type) GetFlowConfig()
    {
        return (FlowType.Oil, FlowType.Kerosene);
    }
}
