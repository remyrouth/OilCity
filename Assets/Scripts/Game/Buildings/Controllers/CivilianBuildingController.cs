using Game.Events;
using System.Collections.Generic;
using UnityEngine;

public sealed class CivilianBuildingController : BuildingController<BuildingScriptableObject>
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Vector2 maxRandomOffset;
    private void Awake()
    {
        _spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        _spriteRenderer.flipX = Random.value < 0.5f;

        _spriteRenderer.transform.localPosition = 
            new Vector2(maxRandomOffset.x * Random.Range(-1f, 1f), maxRandomOffset.y * Random.Range(-1f, 1f));

        BuildingEvents.OnCivilianSpawn();
    }
    public override List<TileAction> GetActions()
    {
        if(FindObjectsByType<CivilianBuildingController>(FindObjectsSortMode.None).Length <2)
            return new List<TileAction>();
        return base.GetActions();
    }
}
