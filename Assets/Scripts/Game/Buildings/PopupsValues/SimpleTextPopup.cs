using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleTextPopup : SinglePopupValue
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private Image _icon;
    private Color? _baseColor, _baseIconColor;
    public void Initialize(string text, Vector2 pos)
    {
        gameObject.SetActive(true);
        if (_baseColor == null)
            _baseColor = _label.color;
        else
            _label.color = _baseColor!.Value;

        if (_baseIconColor == null)
            _baseIconColor = _icon.color;
        else
            _icon.color = _baseIconColor!.Value;

        transform.position = pos;
        _label.transform.localPosition = Vector3.zero;
        _label.text = text;
        _label.transform.DOLocalMoveY(1, popupTime);
        _label.DOColor(new Color(0, 0, 0, 0), popupTime / 2).SetDelay(popupTime / 2).OnComplete(() => EndAnimation());
        _icon.DOColor(new Color(0, 0, 0, 0), popupTime / 2).SetDelay(popupTime / 2);
    }


}
