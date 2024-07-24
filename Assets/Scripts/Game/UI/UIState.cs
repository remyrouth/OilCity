using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public abstract class UIState : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    protected CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
    }
    public abstract GameState type { get; }
    public virtual void OnEnter() { CanvasGroup.interactable = true; CanvasGroup.alpha = 1; }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { CanvasGroup.interactable = false; CanvasGroup.alpha = 0; }
}
