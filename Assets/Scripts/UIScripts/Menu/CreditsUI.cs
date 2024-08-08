using DG.Tweening;
using UnityEngine;

public class CreditsUI : MenuUIState
{
    [SerializeField] private RectTransform creditsPivot;
    [SerializeField] private float endingY;
    [SerializeField] private CanvasGroup creditsCanvas;
    [SerializeField] private CanvasGroup blackCanvas;
    public override MenuUIStateMachine.MenuUIType type => MenuUIStateMachine.MenuUIType.Credits;
    public override void OnEnter()
    {
        base.OnEnter();
        parallax.overrideMouseX = 0;
        creditsPivot.DOKill();
        creditsPivot.anchoredPosition = Vector2.zero;
        creditsCanvas.alpha = 0;
        blackCanvas.alpha = 0;
        creditsCanvas.DOFade(1, 1).SetDelay(1); 
        blackCanvas.DOFade(1,1.5f);
        blackCanvas.DOFade(0,1).SetDelay(1.5f);
        creditsPivot.DOLocalMoveY(endingY, 40).SetDelay(6);
    }
}
