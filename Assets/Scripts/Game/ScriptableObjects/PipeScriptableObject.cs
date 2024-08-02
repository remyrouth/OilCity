using System.Collections.Generic;
using UnityEngine;

public class PipeScriptableObject : BuildingScriptableObject
{
    [SerializeField] private Mesh m_flowDebugMesh;
    [SerializeField] private List<TileAction> actionList = new List<TileAction>();
    [SerializeField] private GameObject _oilSpillout, _keroseneSpillout;
    [SerializeField] private GameObject m_connection, m_noConnection;
    public override TileObjectController CreateInstance(Vector2Int _)
    {

        // Pipes specifically will go to the tile map to place an object there
        // icon comes from BuildingScriptableObject parent
        // BoardManager.Instance.AddTileToXY(spawn_position, icon);


        // this is pretty icky design, but the PipePlacer script is in charge of setting up/passing the values into the pipe controller.
        // the only convenient way I could think of to get those values into the controller before the initial connection setup is done,
        // so it'll have to do for now.
        //
        // see PipePlacer for why this is an empty gameobject instead of the prefab.
        var go = new GameObject();
        go.name = "PipeSystem #" + go.GetInstanceID();
        var component = go.AddComponent<PipeController>();
        component.SetTileActions(actionList);

#if UNITY_EDITOR
        component.SetDebugMesh(m_flowDebugMesh);
#endif
        component.SetConnectionIndicators(m_connection, m_noConnection);
        component.SetActionPivot();
        component.SetParticleSystems(_oilSpillout, _keroseneSpillout);
        return component;
    }
}
