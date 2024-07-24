using UnityEngine;

public sealed class CivilianBuildingController : BuildingController<BuildingScriptableObject>
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    private void Awake()
    {
        _spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
