using System.Collections.Generic;
using UnityEngine;

public class PipeScriptableObject : BuildingScriptableObject
{
    public override TileObjectController CreateInstance(Vector2Int _)
    {
        var go = Instantiate(prefab);
        go.name = "PipeSystem #" + go.GetInstanceID();
        var component = go.GetComponent<PipeController>();
        return component;
    }
}
