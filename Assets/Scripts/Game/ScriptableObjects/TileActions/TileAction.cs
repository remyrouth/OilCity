using UnityEngine;
public abstract class TileAction : ScriptableObject
{
    [field: SerializeField] public Sprite icon { get; protected set; }
    public abstract void OnClicked(TileObjectController toc);
}
