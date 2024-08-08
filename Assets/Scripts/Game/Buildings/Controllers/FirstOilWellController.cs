using System.Collections.Generic;

public sealed class FirstOilWellController : OilWellController
{
    public override List<TileAction> GetActions()
    {
        var actions = base.GetActions();
        actions.RemoveAt(1);
        return actions;
    }
}
