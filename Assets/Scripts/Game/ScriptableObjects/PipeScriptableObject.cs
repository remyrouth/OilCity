public class PipeScriptableObject : BuildingScriptableObject
{
    public override void BeginBuilding() => BuildingManager.Instance.BeginBuilding(this);
}
