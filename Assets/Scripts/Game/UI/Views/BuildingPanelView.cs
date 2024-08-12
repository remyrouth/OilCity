using UnityEngine;
using DG.Tweening;

public class BuildingPanelView : MonoBehaviour
{
    [SerializeField] private float _openPosX, _closedPosX;
    private RectTransform _rectTransform;

    private const float speed = 400;

    public void Open()
    {
        MoveTo(true);
        TutorialArrow.Instance.Enable();
    }

    public void Close()
    {
        MoveTo(false);
        TutorialArrow.Instance.Disable();
    } 
    private void MoveTo(bool toOpen)
    {
        float desiredX = toOpen ? _openPosX : _closedPosX;

        if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();
        float time = Mathf.Abs(_rectTransform.anchoredPosition.x - desiredX) / speed;
        _rectTransform.DOKill();
        _rectTransform.DOLocalMoveX(desiredX, time).SetEase(Ease.InBack);
    }
}
