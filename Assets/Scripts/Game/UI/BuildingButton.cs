using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BuildingScriptableObject _buildingSO;
    [SerializeField] private TMP_Text _costLabel;
    [SerializeField] private Image _buildingIcon;
    // [SerializeField] private GameObject buildingDescriptionPopup;

    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { _buildingSO.BeginBuilding(); });
        _buildingIcon.sprite = _buildingSO.icon;
        _costLabel.text = _buildingSO.placementCost.ToString();

        // if (buildingDescriptionPopup != null)
        // {
        //     buildingDescriptionPopup.SetActive(false);
        // }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PopupDescriptor.Instance != null) {
            PopupDescriptor.Instance.Show(_buildingSO.description, transform.position);
        } 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (PopupDescriptor.Instance != null)
        {
            PopupDescriptor.Instance.Hide();
        }
    }
}
