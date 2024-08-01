using UnityEngine;

public sealed class TreeController : TileObjectController
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer.flipX = Random.value > 0.5f;
    }

}
