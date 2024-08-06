using UnityEngine;

public abstract class BuildingInfoView<T,T1> : MonoBehaviour
    where T : InfoTileAction
    where T1 : TileObjectController
{
    private const float HOVER_TIME_TO_ENABLE = 0.25f;
    private CanvasGroup _canvas;

    /// <summary>
    /// Used to fetch data from action SO and populate text fields
    /// </summary>
    /// <param name="action"></param>
    public abstract void Initialize(T action,T1 tileController);

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        _canvas.alpha = 0;
        _isFocused = true;
    }
    private float _timer = 0;
    private bool _isFocused = false;
    public virtual void BeginFocus()
    {
        //_isFocused = true;
    }
    public virtual void EndFocus()
    {
        //_isFocused = false;
    }
    private void Update()
    {
        if (!_isFocused)
        {
            _canvas.alpha = Mathf.Clamp01(Mathf.Lerp(_canvas.alpha, -0.2f, Time.deltaTime * 10));
            _canvas.blocksRaycasts = false;
            _canvas.interactable = false;
            _timer = 0;
        }
        else
        {
            if (_timer < HOVER_TIME_TO_ENABLE)
            {
                _timer += Time.deltaTime;
                return;
            }
            _canvas.alpha = Mathf.Clamp01(Mathf.Lerp(_canvas.alpha, 1.2f, Time.deltaTime * 10));
            _canvas.blocksRaycasts = true;
            _canvas.interactable = true;
        }

    }

}
