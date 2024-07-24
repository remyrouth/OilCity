using UnityEngine;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(Button))]
public class BuildingButton : MonoBehaviour
{
    [SerializeField] private BuildingScriptableObject _buildingSO;
    [SerializeField] private TMP_Text _costLabel;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(()=>{_buildingSO.BeginBuilding();});
        GetComponent<Image>().sprite = _buildingSO.icon;
        _costLabel.text = _buildingSO.placementCost.ToString();
    }
}
