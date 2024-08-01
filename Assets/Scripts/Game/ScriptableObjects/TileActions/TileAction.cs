using UnityEngine;
public abstract class TileAction : ScriptableObject
{
    [SerializeField] protected Sprite icon;
    [field: SerializeField] public GameObject prefab { get; protected set; }
    public virtual Sprite GetIcon(TileObjectController toc) => icon;
    public virtual GameObject Create(Transform pivot, float rotation, TileObjectController toc, TileSelector ts)
    {
        var obj = Instantiate(prefab, pivot);
        return obj;
    }
}
