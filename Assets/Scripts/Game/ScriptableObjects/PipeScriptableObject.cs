using UnityEngine;

public class PipeScriptableObject : BuildingScriptableObject
{
    public override TileObjectController CreateInstance(Vector2Int _)
    {
        // this is pretty icky design, but the PipePlacer script is in charge of setting up/passing the values into the pipe controller.
        // the only convenient way I could think of to get those values into the controller before the initial connection setup is done,
        // so it'll have to do for now.
        //
        // see PipePlacer for why this is an empty gameobject instead of the prefab.
        return new GameObject("PipeSystem").AddComponent<PipeController>();
    }
}
