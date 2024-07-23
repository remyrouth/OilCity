using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Payrate Building", menuName = "Building/PayrateBuilding")]

public class PayrateBuildingScriptableObject : BuildingScriptableObject
{
    [field: SerializeField] public int basePayrate { get; private set; }
    [field: SerializeField] public int payrateLevelDelta { get; private set; }

    override
    public TileObjectController CreateInstance()
    {
        GameObject tmpObject = Instantiate(prefab);
        var tileObjectController = tmpObject.GetComponent<BuildingController<PayrateBuildingScriptableObject>>();
        tileObjectController.Initialize(this);
        return tileObjectController;
    }
}
