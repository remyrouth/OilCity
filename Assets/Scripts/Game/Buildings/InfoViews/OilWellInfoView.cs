using TMPro;
using UnityEngine;

public class OilWellInfoView : BuildingInfoView<OilWellInfoTileAction, OilWellController>, ILanguageChangeable
{
    [SerializeField] private TMP_Text _nameLabel, _wagesInfo, _productivityLabel, _oilLeftLabel;
    private OilWellController _focusedOilWell;
    [SerializeField] private LanguageItem currently, oilLeft, wages,perText;
    public override void Initialize(OilWellInfoTileAction action, OilWellController tileController)
    {
        _focusedOilWell = tileController;
        UpdateText();
    }

    public void UpdateText()
    {
        UpdateOilLabel(0);
        _focusedOilWell.OnOilMined += UpdateOilLabel;
        _nameLabel.text = _focusedOilWell.config.buildingName.ToString();

        _productivityLabel.text = _focusedOilWell.CurrentMineRate().ToString();
        WriteOilLeft();
        WriteWagesInfo(_focusedOilWell);
    }
    private void WriteWagesInfo(OilWellController tileController)
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
    private void WriteOilLeft()
    {
        float oilSum = 0;
        for (int i = 0; i < _focusedOilWell.config.size.x; i++)
        {
            for (int j = 0; j < _focusedOilWell.config.size.y; j++)
            {
                Vector2Int pos = _focusedOilWell.Anchor + new Vector2Int(i, j);
                oilSum += BoardManager.Instance.OilEvaluator.GetValueAtPosition(pos.x,pos.y);
            }
        }
        _oilLeftLabel.text = $"{oilLeft}: {(oilSum * 10000).ToString("0.00")}L";
    }
    private void OnDestroy()
    {
        if(_focusedOilWell != null)
            _focusedOilWell.OnOilMined -= UpdateOilLabel;
    }
    private void UpdateOilLabel(float newValue)
    {
        _productivityLabel.text = $"Currently: {(newValue*10000).ToString("0.00")} {perText}";
    }
}
