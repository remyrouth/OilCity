using TMPro;
using UnityEngine;

public class RefineryInfoView : BuildingInfoView<RefineryInfoTileAction, RefineryController>
{
    [SerializeField] private TMP_Text _nameLabel, _descriptionLabel, _productivityLabel, _wageLabel;
    private RefineryController _focusedRefinery;
    public override void Initialize(RefineryInfoTileAction action, RefineryController tileController)
    {
        _focusedRefinery = tileController;
        UpdateKeroseneLabel(0);
        _focusedRefinery.OnKeroseneProduced += UpdateKeroseneLabel;
        _nameLabel.text = tileController.config.buildingName.ToString();
        _descriptionLabel.text = tileController.config.description.ToString();
        _wageLabel.text = tileController.CurrentPaymentMode.ToString();
    }
    private void OnDestroy()
    {
        if (_focusedRefinery != null)
            _focusedRefinery.OnKeroseneProduced -= UpdateKeroseneLabel;
    }
    private void UpdateKeroseneLabel(float newValue)
    {
        _productivityLabel.text = $"Currently: {(newValue * 10000).ToString("0.00")} kerosene/tick";
    }
}
