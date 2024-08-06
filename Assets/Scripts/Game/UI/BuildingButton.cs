using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IGraphicsChangeable
{
    [SerializeField] private BuildingScriptableObject _buildingSO;
    [SerializeField] private TMP_Text _costLabel;
    [SerializeField] private Image _buildingIcon;
    [SerializeField] private PopupDescriptorView _descriptorView;
    private Image _highlightImage;

    private Color _originalColor;
    private Color _highlightColor;
    private bool _isFlickering;

    public void Awake()
    {
        _highlightImage = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(() => { _buildingSO.BeginBuilding(); });
        ChangeGraphics(GraphicsSwapperManager.SetNewer);
        _costLabel.text = _buildingSO.placementCost.ToString();

        if (_highlightImage != null)
        {
            _originalColor = _highlightImage.color;
            _highlightColor = new Color(1f, 1f, 0f, 0.75f);
            _isFlickering = false;
        }
    }

    public void ChangeGraphics(bool pickNewer)
    {
        _buildingIcon.sprite = pickNewer? _buildingSO.icon: _buildingSO.old_icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _descriptorView?.BeginFocus(_buildingSO, GetComponent<RectTransform>().anchoredPosition);

        SoundManager.Instance.SelectBuildingSFXTrigger();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptorView?.EndFocus();
    }

    public void ToggleHighlight()
    {
        if (_highlightImage != null)
        {
            if (!_isFlickering)
            {
                _highlightImage.DOColor(_highlightColor, 1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
                _isFlickering = true;
            }
            else
            {
                _highlightImage.DOKill();
                _highlightImage.color = _originalColor; 
                _isFlickering = false;
            }
            
        }
    }
}