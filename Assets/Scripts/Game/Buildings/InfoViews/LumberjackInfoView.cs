using TMPro;
using UnityEngine;

public class LumberjackInfoView : BuildingInfoView<LumberjackInfoTileAction, WoodCutterController>,ILanguageChangeable
{
    [SerializeField] private TMP_Text _nameLabel, _wagesInfo, _productivityLabel, _radiusLabel;
    private WoodCutterController _focusedWoodcutter;
    [SerializeField] private LanguageItem workers, radius, wages;
    public override void Initialize(LumberjackInfoTileAction action, WoodCutterController tileController)
    {
        _focusedWoodcutter = tileController;
        UpdateText();
        if (_focusedWoodcutter != null)
            _focusedWoodcutter.GetComponent<BuildingRangeShower>().ShowRadius();
    }
    public override void EndFocus()
    {
        if (_focusedWoodcutter != null)
            _focusedWoodcutter.GetComponent<BuildingRangeShower>().HideRadius();
    }

    public void UpdateText()
    {
        _nameLabel.text = _focusedWoodcutter.config.buildingName.ToString();
        _productivityLabel.text = $"{_focusedWoodcutter.WorkersCount} {workers}";
        _radiusLabel.text = $"{radius}: {_focusedWoodcutter.Range}";
        WriteWagesInfo(_focusedWoodcutter);
    }
    private void WriteWagesInfo(WoodCutterController tileController)
    {
        string text = $"{wages}:\n";
        for (int i = 0; i < 3; i++)
        {
            if (i == (int)tileController.CurrentPaymentMode)
                text += '>';
            text += (tileController.config.basePayrate + tileController.config.payrateLevelDelta * i).ToString() + " z³\n";
        }
        _wagesInfo.text = text;
    }
}
