using UnityEngine;

public class CivilianBuildingOutline : BuildingsOutline
{
    [SerializeField] private SpriteRenderer source;
    protected override void Start()
    {
        outline.sprite = source.sprite;
        base.Start();
    }
}
