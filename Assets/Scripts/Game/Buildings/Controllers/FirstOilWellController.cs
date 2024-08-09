using System.Collections.Generic;
using UnityEngine;

public sealed class FirstOilWellController : OilWellController
{
    [SerializeField] private GameObject ignacy;
    public override List<TileAction> GetActions()
    {
        var actions = base.GetActions();
        actions.RemoveAt(1);
        return actions;
    }
    public void KillIgnacy()
    {
        if (ignacy != null)
            Destroy(ignacy);
    }
}
