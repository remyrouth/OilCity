using UnityEngine;

public abstract class SinglePopupValue : MonoBehaviour
{
    [SerializeField] private PopupValuesPool.PopupValueType _type;
    [SerializeField] protected float popupTime = 4;
    public PopupValuesPool.PopupValueType type => _type;
    protected void EndAnimation()
    {
        PopupValuesPool.Instance.GiveAwayToPool(this);
    }
    private bool visible = true;
    private void OnBecameVisible() => visible = true;
    private void OnBecameInvisible() => visible = false;
    private CanvasGroup _canvasGroup;
    private void Awake() => _canvasGroup = GetComponent<CanvasGroup>();
    private void LateUpdate()
    {
        if (!visible)
            return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(ControlManager.Instance.RetrieveMousePosition());
        float distance = (new Vector2(transform.position.x, transform.position.y) - mousePos).sqrMagnitude / 40;
        _canvasGroup.alpha = 0.2f + Mathf.Clamp(1 - distance, 0, 0.8f);


    }


}
