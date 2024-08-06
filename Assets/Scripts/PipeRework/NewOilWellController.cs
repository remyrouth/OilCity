using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NewOilWellController : PayrateBuildingController, INewFlowable
{
    public const float BASE_OIL_RATE = 0.01f;
    private const float OIL_RATE_DELTA = 0.002f;

    [SerializeField] private ParticleSystem _spilloutEffect;
    private int _tickTimer;
    private int PaymentTimer => 5;
    public event Action<float> OnOilMined;

    private TreeRelationship m_tr = new(0, 1);

    public float SendFlow()
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
        return amountMined;
    }

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
                    // ping the pipe? display a notif that wells cant have inputs?
                }
            }
        }
    }

    /// <summary>
    /// An oil well can only handle output pipes.
    /// </summary>
    /// <returns></returns>
    public (bool can_input, bool can_output) GetInOutConfig() => (false, !m_tr.HasMaxParents());

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
        Debug.LogWarning("Oil well has overflowed " + flow);
    }

    public TreeRelationship GetTreeRelationship() => m_tr;

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

    public void UpdateConnections(ISet<INewFlowable> seen)
    {
        if (seen.Contains(this)) return;

        seen.Add(this);

        // bit dumb to continue the recursion, but it won't break anything
        // this is so because this method is invoked by relationships, of which an oil
        // well only has one of. Therefore, the only place it can send the signal to
        // is the place it got it from.
        // 
        // but whatever. it keeps the code tidy to have a pattern, so i'll keep it.

        foreach (var child in m_tr.GetChildren()) child.UpdateConnections(seen);
        foreach (var parent in m_tr.GetParents()) parent.UpdateConnections(seen);
    }
}
