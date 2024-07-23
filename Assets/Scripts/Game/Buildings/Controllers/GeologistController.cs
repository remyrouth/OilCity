using UnityEngine;
using System.Collections;
public sealed class GeologistController : AOEBuildingController
{
    public override int TickNumberInterval => 1;

    public override int Range => 1;

    private int GetNumberOfSearchingPoints() => 3;

    private IEnumerator SearchForOil()
    {
        yield return null;
    }

    public override void OnTick()
    {
    }
}
