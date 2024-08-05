using DG.Tweening;
using UnityEngine;

public class SimpleIconPopup : SinglePopupValue
{
    public void Initialize(Vector2 pos)
    {
        transform.position = pos;
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.1f);
        transform.DOScale(0, 0.1f).SetDelay(popupTime).OnComplete(() => EndAnimation());
    }
}
