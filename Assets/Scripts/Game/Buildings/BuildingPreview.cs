using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    public virtual void SetState(bool IsPossible)
    {
        _renderer.color = new Color(1, 1, 1, IsPossible ? 1 : 0.75f);
    }
}
