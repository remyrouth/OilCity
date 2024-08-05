using UnityEngine;

public class NewPipeScriptableObject : BuildingScriptableObject
{
    public override TileObjectController CreateInstance(Vector2Int spawn_position)
    {
        GameObject tmpObject = Instantiate(prefab);
        tmpObject.name = "PipeSystem #" + tmpObject.GetInstanceID();
        return tmpObject.GetComponent<PipeController>();
    }
}
