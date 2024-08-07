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
        _flowrateDesc.text = $"{max}: {(_focusedRefinery.GetBaseRefineryFlowrate() * 10000).ToString("0.00")} {perText}";
        WriteWagesInfo(_focusedRefinery);
    }

    private void OnDestroy()
    {
        if (_focusedRefinery != null)
            _focusedRefinery.OnKeroseneProduced -= UpdateKeroseneLabel;
    }
    private void UpdateKeroseneLabel(float newValue)
    {
        _productivityLabel.text = $"{currently}: {(newValue * 10000).ToString("0.00")} {perText}";
    }
    private void WriteWagesInfo(RefineryController tileController)
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
