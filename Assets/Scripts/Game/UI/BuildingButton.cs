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


    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { _buildingSO.BeginBuilding(); });
        _buildingIcon.sprite = _buildingSO.icon;
        _costLabel.text = _buildingSO.placementCost.ToString();

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _descriptorView.BeginFocus(_buildingSO, GetComponent<RectTransform>().anchoredPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptorView.EndFocus();
    }
}