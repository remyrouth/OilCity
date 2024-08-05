using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class MaterialSwapper : MonoBehaviour,IGraphicsChangeable
{
    [SerializeField] private Material newer, older;
    private SpriteRenderer _renderer;
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        ChangeGraphics(GraphicsSwapperManager.SetNewer);
    }
    public void ChangeGraphics(bool pickNewer)
    {
        _renderer.material = pickNewer ? newer : older;
    }
}
