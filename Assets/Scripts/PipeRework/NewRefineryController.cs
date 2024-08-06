using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NewRefineryController : PayrateBuildingController, INewFlowable
{
    private float baseRefineryFlowrate;
    private float keroseneMultiplier;

    [SerializeField] private ParticleSystem _spilloutEffect;
    [SerializeField] private ParticleSystem[] _workingSmokeEffects;
    private int _tickTimer;
    private int PaymentTimer => 5;
    public bool IsWorking { get; private set; } = false;
    public event Action<float> OnKeroseneProduced;

    private TreeRelationship m_tr = new(99, 1);

    protected override void CreateInitialConnections(Vector2Int with_position)
    {
        var peripherals = BoardManager.Instance.GetPeripheralTileObjectsForBuilding(with_position, config.size);

        foreach (var (peripheral_to, tile) in peripherals)
        {
            if (tile.TryGetComponent<NewPipeController>(out var pipe))
            {
                if (pipe.DoesPipeSystemReceiveInputFromTile(peripheral_to))
                {
                    /*
                    if (m_output != null)
                    {
                        // more than one output pipe discovered
                        // ping the pipe? display a notif that this pipe isnt going to be used?
                        // TODO

                        return;
                    }
                    */

                    m_tr.AddTentative(pipe, Relation.Parent);

                    // pipe.AddChild(this);
                    // SetParent(pipe);
                }
                else if (pipe.DoesPipeSystemOutputToTile(peripheral_to))
                {
                    m_tr.AddTentative(pipe, Relation.Child);

                    // pipe.SetParent(this);
                    // AddChild(pipe);
                }
            }
        }
    }

    /// <summary>
    /// Refineries can input and output fluid to pipes.
    /// </summary>
    /// <returns></returns>
    public (bool can_input, bool can_output) GetInOutConfig() => (true, !m_tr.HasMaxParents());

    public float SendFlow()
    {
        float OilSum = 0;
        keroseneMultiplier = GetKeroseneMultiplier();
        foreach (var child in m_tr.GetChildren())
        {
            var received = child.SendFlow();

            /*
            if (received.type == FlowType.Kerosene)
            {
                Debug.LogWarning("Refinery just received Kerosene!!!", gameObject);
                continue;
            }
            */

            IsWorking = true;
            OilSum += received * keroseneMultiplier;
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
        return OilSum;
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

    public TreeRelationship GetTreeRelationship() => m_tr;

    public void OnTick()
    {
        _tickTimer++;
        var flow = SendFlow();
        if (_tickTimer == PaymentTimer)
        {
            _tickTimer = 0;
            PayWorkers();
        }
        if (flow > 0)
            _spilloutEffect.Play();
        else
            _spilloutEffect.Stop();
        if (flow == 0)
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

    public void UpdateConnections(ISet<INewFlowable> seen)
    {
        if (seen.Contains(this)) return;

        seen.Add(this);

        foreach (var child in m_tr.GetChildren()) child.UpdateConnections(seen);
        foreach (var parent in m_tr.GetParents()) parent.UpdateConnections(seen);
    }
}
