using TMPro;
using UnityEngine;

public class GeologistInfoView : BuildingInfoView<GeologistInfoTileAction, GeologistController>
{
    [SerializeField] private TMP_Text _nameLabel, _descriptionLabel, _productivityLabel, _wageLabel;
    private GeologistController _focusedGeologist;
    public override void Initialize(GeologistInfoTileAction action, GeologistController tileController)
    {
        _focusedGeologist = tileController;
        _productivityLabel.text = $"{tileController.activeWorkerAmount} active workers";
        _nameLabel.text = tileController.config.buildingName.ToString();
        _descriptionLabel.text = tileController.config.description.ToString();
        _wageLabel.text = tileController.CurrentPaymentMode.ToString();
        if (_focusedGeologist != null)
            _focusedGeologist.GetComponent<BuildingRangeShower>().ShowRadius();
    }
    public override void EndFocus()
    {
        if (_focusedGeologist != null)
            _focusedGeologist.GetComponent<BuildingRangeShower>().HideRadius();
    }
}
