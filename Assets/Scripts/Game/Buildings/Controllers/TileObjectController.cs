using System.Collections.Generic;
using UnityEngine;

public class TileObjectController : MonoBehaviour
{
    public virtual List<TileAction> GetActions() => new List<TileAction> { };
    public virtual Vector2Int size => new Vector2Int(1, 1);
    public Vector2Int Anchor => new Vector2Int((int)transform.position.x, (int)transform.position.y);
}
