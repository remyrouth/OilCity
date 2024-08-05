using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorSwapper : MonoBehaviour, IGraphicsChangeable
{
    [SerializeField] private AnimatorOverrideController newer, older;
    private Animator _anim;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    private void Start()
    {
        ChangeGraphics(GraphicsSwapperManager.SetNewer);
    }
    public void ChangeGraphics(bool pickNewer)
    {
        _anim.runtimeAnimatorController = pickNewer ? newer : older;
    }
}
