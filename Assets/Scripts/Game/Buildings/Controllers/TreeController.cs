using Unity.VisualScripting;
using UnityEngine;

public sealed class TreeController : TileObjectController
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public override System.Action GetCreateAction(Vector2Int pos) => () => BoardManager.Instance.CreateTree(pos);
    private void Awake()
    {
        _spriteRenderer.flipX = Random.value > 0.5f;
    }

}
