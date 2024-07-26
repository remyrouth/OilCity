using UnityEngine;

public sealed class TreeController : TileObjectController
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    private void Awake()
    {
        _spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }

}
