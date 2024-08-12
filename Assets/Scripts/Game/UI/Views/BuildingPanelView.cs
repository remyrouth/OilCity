using UnityEngine;
using DG.Tweening;

public class BuildingPanelView : MonoBehaviour
{
    [SerializeField] private float _openPosX, _closedPosX;
    private RectTransform _rectTransform;
    [SerializeField] private GameObject tutorialArrow;

    private const float speed = 400;

    public void Open()
    {
        if(tutorialArrow is not null)
        {
            TutorialArrow.Instance.Enable();
        }
        MoveTo(true);
    }

    public void Close()
    {
        if(tutorialArrow is not null)
        {
            TutorialArrow.Instance.Disable();
        }
        MoveTo(false);
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
