using TMPro;
using UnityEngine;

public class OilWellInfoView : BuildingInfoView<OilWellInfoTileAction, OilWellController>
{
    [SerializeField] private TMP_Text _nameLabel, _descriptionLabel, _productivityLabel,_wageLabel;
    private OilWellController _focusedOilWell;
    public override void Initialize(OilWellInfoTileAction action, OilWellController tileController)
    {
        _focusedOilWell = tileController;
        UpdateOilLabel(0);
        _focusedOilWell.OnOilMined += UpdateOilLabel;
        _nameLabel.text = tileController.config.buildingName.ToString();
        _descriptionLabel.text = tileController.config.description.ToString();
        _wageLabel.text = tileController.CurrentPaymentMode.ToString();
    }
    private void OnDestroy()
    {
        if(_focusedOilWell != null)
            _focusedOilWell.OnOilMined -= UpdateOilLabel;
    }
    private void UpdateOilLabel(float newValue)
    {
        _productivityLabel.text = $"Currently: {(newValue*10000).ToString("0.00")} oil/tick";
    }
}
