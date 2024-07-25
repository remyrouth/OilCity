using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public sealed class WoodCutterController : AOEBuildingController
{
    public override int TickNumberInterval => 10;

    public override int Range => 4;

    private int _tickTimer = 0;

    private bool _active = true;

    public override void OnTick()
    {
        _tickTimer++;
        if (_tickTimer > TickNumberInterval)
        {
            _tickTimer = 0;
            if (!_active) return;
            InvokeAction();
        }
    }
    /// <summary>
    /// Checks each tile in range of the building, if its a tree, adds it to a list from which it later chooses a random one to destroy.
    /// </summary>
    public void InvokeAction()
    {
        List<TileObjectController> trees = new List<TileObjectController>();
        foreach (Vector2Int pos in GetTilesInRange())
        {
            if (!BoardManager.Instance.IsTileOccupied(pos)) continue;
            if (!BoardManager.Instance.tileDictionary[pos].TryGetComponent<TreeController>(out TreeController tree)) continue;
            trees.Add(tree);
        }
        if (trees.Any())
        {
            BoardManager.Instance.Destroy(trees[Random.Range(0, trees.Count)]);
            Debug.Log("Destroying random tree...");
        }
        else
        {
            _active = false;
        }
    }
}
