using System.Collections.Generic;
using UnityEngine;

public class TileObjectController : MonoBehaviour
{
    public virtual List<TileAction> GetActions() => new List<TileAction> { };
}
