using Game.Events;
using System.Collections.Generic;
using UnityEngine;

public sealed class CivilianBuildingController : BuildingController<BuildingScriptableObject>, IGraphicsChangeable
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] old_sprites;
    [SerializeField] private Vector2 maxRandomOffset;

    private int _gfxSeed;

    public override void Initialize(BuildingScriptableObject config, Vector2Int spawn_position)
    {
        base.Initialize(config, spawn_position);
        _gfxSeed = Random.Range(0, sprites.Length * old_sprites.Length);
        ChangeGraphics(GraphicsSwapperManager.SetNewer);
        _spriteRenderer.flipX = Random.value < 0.5f;

        _spriteRenderer.transform.localPosition = config.size / 2 +
                                                  new Vector2(maxRandomOffset.x * Random.Range(-1f, 1f), maxRandomOffset.y * Random.Range(-1f, 1f));

        BuildingEvents.OnCivilianSpawn();
    }
    public override List<TileAction> GetActions()
    {
        if (FindObjectsByType<CivilianBuildingController>(FindObjectsSortMode.None).Length < 2)
            return new List<TileAction>();
        return base.GetActions();
    }
    protected override void OnDestroy()
    {
        CivilianCityManager.Instance.NumOfBuildings--;
        base.OnDestroy();
    }

    public void ChangeGraphics(bool pickNewer)
    {
        Sprite[] sprites = pickNewer ? this.sprites : old_sprites;
        _spriteRenderer.sprite = sprites[_gfxSeed % sprites.Length];
    }
}
