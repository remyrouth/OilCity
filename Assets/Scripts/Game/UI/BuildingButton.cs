using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BuildingScriptableObject _buildingSO;
    [SerializeField] private TMP_Text _costLabel;
    [SerializeField] private Image _buildingIcon;
    [SerializeField] private PopupDescriptorView _descriptorView;
    [SerializeField] private Image _highlightImage;

    private Color _originalColor;
    private Color _highlightColor = Color.black;


    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { _buildingSO.BeginBuilding(); });
        _buildingIcon.sprite = _buildingSO.icon;
        _costLabel.text = _buildingSO.placementCost.ToString();

        if (_highlightImage != null)
        {
            _originalColor = _highlightImage.color;
            _highlightColor = new Color(1f, 1f, 0f, 0.5f);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _descriptorView.BeginFocus(_buildingSO, GetComponent<RectTransform>().anchoredPosition);
        StartFlicker();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptorView.EndFocus();
        StopFlicker();
    }

    private void StartFlicker()
    {
        if (_highlightImage != null)
        {
            _highlightImage.DOColor(_highlightColor, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    private void StopFlicker()
    {
        if (_highlightImage != null)
        {
            _highlightImage.DOKill();
            _highlightImage.color = _originalColor;
        }
    }
}