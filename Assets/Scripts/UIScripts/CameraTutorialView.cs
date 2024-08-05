using DG.Tweening;
using UnityEngine;

public class CameraTutorialView : MonoBehaviour
{
    private float _timer;
    private Vector3 _cameraPos;
    private CameraController _camController;
    private CanvasGroup _tutorialCanvas;
    private const float TIME_BEFORE_TUTORIAL = 10;
    private void Start()
    {
        _timer = Time.time + TIME_BEFORE_TUTORIAL;
        _tutorialCanvas = GetComponent<CanvasGroup>();
        _camController = FindAnyObjectByType<CameraController>();
    }
    private bool _alreadyShowing = false;
    private void LateUpdate()
    {
        if (_camController.LastPlayerDrag.sqrMagnitude > 0.1)
        {
            _tutorialCanvas.DOKill();
            _tutorialCanvas.DOFade(0, _tutorialCanvas.alpha * 0.2f);
            enabled = false;
            return;
        }
        if (Time.time > _timer && !_alreadyShowing)
        {
            _alreadyShowing = true;
            _tutorialCanvas.DOKill();
            _tutorialCanvas.DOFade(1, (1 - _tutorialCanvas.alpha) * 0.2f);
        }
    }


}
