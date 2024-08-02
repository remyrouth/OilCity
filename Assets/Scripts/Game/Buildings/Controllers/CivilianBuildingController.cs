using System.Collections.Generic;
using UnityEngine;

public sealed class CivilianBuildingController : BuildingController<BuildingScriptableObject>
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    private void Awake()
    {
        _spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }
    public override List<TileAction> GetActions()
    {
        if(FindObjectsByType<CivilianBuildingController>(FindObjectsSortMode.None).Length <2)
            return new List<TileAction>();
        return base.GetActions();
    }
}
