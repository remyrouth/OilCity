using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HighlightUI : Singleton<HighlightUI>
{
    [SerializeField] private List<Image> highlights;
    private bool _highlightEnabled;
    private Color _originalColor = new Color(1f, 1f, 0f, 0f);
    private Color _highlightColor = new Color(1f, 1f, 0f, 0.5f);
    
    private void Awake()
    {
        _highlightEnabled = false;
    }

    public void ToggleHighlight(int highlightIndex)
    {
        if (!_highlightEnabled)
        {

            highlights[highlightIndex].DOColor(_highlightColor, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
            _highlightEnabled = true;
        }
        else
        {
            highlights[highlightIndex].DOKill();
            highlights[highlightIndex].color = _originalColor; 
            _highlightEnabled = false;
        }
    }
}
