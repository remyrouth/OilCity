using System;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectController : MonoBehaviour
{
    public virtual List<TileAction> GetActions() => new List<TileAction> { };
    public virtual Vector2Int size => new Vector2Int(1, 1);
    public Vector2Int Anchor => new Vector2Int((int)transform.position.x, (int)transform.position.y);

    [SerializeField] protected Vector2 _actionsPivot;
    public Vector2 ActionsPivot => Anchor + _actionsPivot;
    public virtual bool CheckIfDestroyable() => false;
    public virtual Action GetCreateAction(Vector2Int pos) => null;
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(ActionsPivot, 0.25f);
    }
#endif
}
