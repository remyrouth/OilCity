using TMPro;
using UnityEngine;

public class LumberjackInfoView : BuildingInfoView<LumberjackInfoTileAction, WoodCutterController>
{
    [SerializeField] private TMP_Text _nameLabel, _descriptionLabel, _productivityLabel, _wageLabel;
    private WoodCutterController _focusedWoodcutter;
    public override void Initialize(LumberjackInfoTileAction action, WoodCutterController tileController)
    {
        _focusedWoodcutter = tileController;
        _productivityLabel.text = $"{tileController.WorkersCount} workers";
        _nameLabel.text = tileController.config.buildingName.ToString();
        _descriptionLabel.text = tileController.config.description.ToString();
        _wageLabel.text = tileController.CurrentPaymentMode.ToString();
        if (_focusedWoodcutter != null)
            _focusedWoodcutter.GetComponent<BuildingRangeShower>().ShowRadius();
    }
    public override void EndFocus()
    {
        if (_focusedWoodcutter != null)
            _focusedWoodcutter.GetComponent<BuildingRangeShower>().HideRadius();
    }
}
