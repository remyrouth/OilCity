using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public abstract class MenuUIState : MonoBehaviour
{
    [SerializeField] protected ParallaxEffect parallax;
    [SerializeField] protected RectTransform background;
    private CanvasGroup _canvas;
    protected CanvasGroup Canvas
    {
        get
        {
            if (_canvas == null)
                _canvas = GetComponent<CanvasGroup>();
            return _canvas;
        }
    }
    public abstract MenuUIStateMachine.MenuUIType type { get; }
    public virtual void OnEnter()
    {
        Canvas.alpha = 1.0f;
        Canvas.interactable = true;
        Canvas.blocksRaycasts = true;
    }
    public virtual void OnExit()
    {
        Canvas.alpha = 0f;
        Canvas.interactable = false;
        Canvas.blocksRaycasts = false;
    }
    public virtual void OnUpdate() { }
}
