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
        _productivityLabel.text = $"{workers}<color=#594331> {_focusedWoodcutter.WorkersCount}";
        _radiusLabel.text = $"{radius}: <color=#594331>{_focusedWoodcutter.Range}";
        WriteWagesInfo(_focusedWoodcutter);
    }
    private void WriteWagesInfo(WoodCutterController tileController)
    {
        string text = $"{wages}:\n<color=#594331>";
        for (int i = 0; i < 3; i++)
        {
            if (i == (int)tileController.CurrentPaymentMode)
                text += "<u>"+'>';
            text += (tileController.config.basePayrate + tileController.config.payrateLevelDelta * i).ToString() + " zł\n";
            if (i == (int)tileController.CurrentPaymentMode)
                text += "</u>";
        }
        _wagesInfo.text = text;
    }
}
