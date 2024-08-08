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
        _focusedOilWell.OnOilMined += UpdateOilLabel;
        UpdateText();
    }
    public void UpdateText()
    {
        UpdateOilLabel(0);
        _nameLabel.text = _focusedOilWell.config.buildingName.ToString();

        WriteOilLeft();
        WriteWagesInfo(_focusedOilWell);
    }
    private void WriteWagesInfo(OilWellController tileController)
    {
        string text = $"{wages}:\n";
        for (int i = 0; i < 3; i++)
        {
            if (i == (int)tileController.CurrentPaymentMode)
                text += "<color=#594331>" + "<u>" + '>';
            text += "<color=#594331>" + (tileController.config.basePayrate + tileController.config.payrateLevelDelta * i).ToString() + " zł\n";
            if (i == (int)tileController.CurrentPaymentMode)
                text += "</u>";
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
        _oilLeftLabel.text = $"{oilLeft}: <color=#594331> {(oilSum * 10000).ToString("0.00")}L";
    }
    private void OnDestroy()
    {
        if(_focusedOilWell != null)
            _focusedOilWell.OnOilMined -= UpdateOilLabel;
    }
    private void UpdateOilLabel(float newValue)
    {
        WriteOilLeft();
        _productivityLabel.text = $"{currently}: <color=#594331>{(newValue*10000).ToString("0.00")} {perText}";
    }
}
