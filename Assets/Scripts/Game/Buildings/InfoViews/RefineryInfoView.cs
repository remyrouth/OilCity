using TMPro;
using UnityEngine;

public class RefineryInfoView : BuildingInfoView<RefineryInfoTileAction, RefineryController>, ILanguageChangeable
{
    [SerializeField] private TMP_Text _nameLabel, _wagesInfo, _productivityLabel, _flowrateDesc;
    private RefineryController _focusedRefinery;
    [SerializeField] private LanguageItem currently, max, wages, perText;
    public override void Initialize(RefineryInfoTileAction action, RefineryController tileController)
    {
        _focusedRefinery = tileController;
        UpdateText();
    }

    public void UpdateText()
    {
        UpdateKeroseneLabel(0);
        _focusedRefinery.OnKeroseneProduced += UpdateKeroseneLabel;
        _nameLabel.text = _focusedRefinery.config.buildingName.ToString();
        _flowrateDesc.text = $"{max}:\n<color=#594331>{(_focusedRefinery.GetBaseRefineryFlowrate() * 10000).ToString("0.00")} {perText}";
        WriteWagesInfo(_focusedRefinery);
    }

    private void OnDestroy()
    {
        if (_focusedRefinery != null)
            _focusedRefinery.OnKeroseneProduced -= UpdateKeroseneLabel;
    }
    private void UpdateKeroseneLabel(float newValue)
    {
        _productivityLabel.text = $"{currently}:\n<color=#594331>{(newValue * 10000).ToString("0.00")} {perText}";
    }
    private void WriteWagesInfo(RefineryController tileController)
    {
        string text = $"{wages}:\n";
        for (int i = 0; i < 3; i++)
        {
            if (i == (int)tileController.CurrentPaymentMode)
                text += "<color=#594331><u>" + '>';
            text += "<color=#594331>" + (tileController.config.basePayrate + tileController.config.payrateLevelDelta * i).ToString() + " zł\n";
            if (i == (int)tileController.CurrentPaymentMode)
                text += "</u>";
        }
        _wagesInfo.text = text;
    }
}
