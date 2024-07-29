using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class BuildingButton : MonoBehaviour, IPointerEnterHandler;
{
    [SerializeField] private BuildingScriptableObject _buildingSO;
    [SerializeField] private TMP_Text _costLabel;
    [SerializeField] private Image _buildingIcon;
    [SerializeField] private GameObject popupDescription;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { _buildingSO.BeginBuilding(); });
        _buildingIcon.sprite = _buildingSO.icon;
        _costLabel.text = _buildingSO.placementCost.ToString();
    }
}
