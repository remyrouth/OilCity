using UnityEngine;
using TMPro;

public class PopupDescriptorView : MonoBehaviour
{
    private const float HOVER_TIME_TO_ENABLE = 0.25f;
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private TMP_Text _descriptionText;
    private RectTransform _rect;
    private CanvasGroup _canvas;

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        _rect = GetComponent<RectTransform>();
    }
    private float _timer = 0;
    private BuildingScriptableObject _currentFocus = null;
    public void BeginFocus(BuildingScriptableObject data, Vector3 position)
    {
        _rect.anchoredPosition = position;
        _currentFocus = data;
        _nameLabel.text = data.buildingName.ToString();
        _descriptionText.text = data.description.ToString();
    }
    public void EndFocus()
    {
        _currentFocus = null;
    }
    private void Update()
    {
        if (_currentFocus == null)
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