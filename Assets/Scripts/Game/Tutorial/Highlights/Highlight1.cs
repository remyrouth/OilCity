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
    private Image _highlightImage;
    private Color _originalColor;
    private Color _highlightColor = new Color(1f, 1f, 0f, 0.5f);

    private void Awake()
    {
        _highlightImage = GetComponent<Image>();
        if (_highlightImage != null)
        {
            _originalColor = _highlightImage.color;
        }
        else
        {
            Debug.LogError("Highlight Image not found on HIghlight1");
        }
    }

    public void StartFlicker()
    {
        if (_highlightImage != null)
        {
            _highlightImage.DOColor(_highlightColor, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        }
    }

    public void StopFlicker()
    {
        if (_highlightImage != null)
        {
            _highlightImage.DOKill();
            _highlightImage.color = _originalColor;
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}