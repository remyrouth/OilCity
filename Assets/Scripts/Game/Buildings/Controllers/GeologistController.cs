using UnityEngine;
public sealed class GeologistController : AOEBuildingController
{
    public override int TickNumberInterval => 1;

    public override int Range => 1;

    protected override void InvokeAction()
    {
        Debug.Log("Searching for oil...");
    }
}
