using UnityEngine;
public sealed class WoodCutterController : AOEBuildingController
{
    public override int TickNumberInterval => 1;

    public override int Range => 1;

    protected override void InvokeAction()
    {
        Debug.Log("Destroying random tree...");
    }
}
