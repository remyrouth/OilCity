using UnityEngine;

public class AOEBuildingScriptableObject : PayrateBuildingScriptableObject
{
    [field: SerializeField] public int BaseRange { get; private set; }
    public override TileObjectController CreateInstance(Vector2Int with_position)
    {
        var obj = base.CreateInstance(with_position);
        obj.GetComponent<AOEBuildingController>().SetBaseRange(BaseRange);
        return obj;
    }
}
