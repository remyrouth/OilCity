using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Building/Building")]
public class BuildingScriptableObject : ScriptableObject
{
    [field: SerializeField] public int placementCost { get; private set; }
    [field: SerializeField] public int removalCost { get; private set; }
    [field: SerializeField] public int removalSatisfactionCost { get; private set; }
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public GameObject prefab { get; private set; }
    [field: SerializeField] public GameObject previewPrefab { get; private set; }
    [field: SerializeField] public Vector2Int size { get; private set; }
    [field: SerializeField] public LanguageItem buildingName { get; private set; }
    [field: SerializeField] public LanguageItem description { get; private set; }

    public virtual TileObjectController CreateInstance(Vector2Int spawn_position)
    {
        GameObject tmpObject = Instantiate(prefab);
        var tileObjectController = tmpObject.GetComponent<BuildingController<BuildingScriptableObject>>();
        tileObjectController.Initialize(this, spawn_position);

        return tileObjectController;
    }
    public virtual void BeginBuilding() => BuildingManager.Instance.BeginBuilding(this);
}
