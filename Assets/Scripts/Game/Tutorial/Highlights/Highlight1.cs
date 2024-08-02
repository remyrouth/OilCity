// using UnityEngine;
// using DG.Tweening;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;


public class Highlight1 : MonoBehaviour
{
    [SerializeField]
    private Image _highlightImage;
    private Color _highlightColor = new Color(1f, 1f, 0f, 0.5f);

    public void StartFlicker(Transform Button)
    {
        transform.position = Button.position;
        if (_highlightImage != null)
        {
            _highlightImage.enabled = true;
            _highlightImage.DOColor(_highlightColor, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        }
    }

    public void StopFlicker()
    {
        if (_highlightImage != null)
        {
            _highlightImage.enabled = false;
            _highlightImage.DOKill();
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}