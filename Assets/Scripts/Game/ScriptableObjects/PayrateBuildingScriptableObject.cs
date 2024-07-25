using UnityEngine;
[CreateAssetMenu(fileName = "New Payrate Building", menuName = "Building/PayrateBuilding")]

public class PayrateBuildingScriptableObject : BuildingScriptableObject
{
    [field: SerializeField] public int basePayrate { get; private set; }
    [field: SerializeField] public int payrateLevelDelta { get; private set; }

    public override TileObjectController CreateInstance(Vector2Int with_position)
    {
        GameObject tmpObject = Instantiate(prefab);
        var tileObjectController = tmpObject.GetComponent<BuildingController<PayrateBuildingScriptableObject>>();
        tileObjectController.Initialize(this, with_position);
        return tileObjectController;
    }
}
