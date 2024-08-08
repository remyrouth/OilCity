using TMPro;
using UnityEngine;

public class GeologistInfoView : BuildingInfoView<GeologistInfoTileAction, GeologistController>,ILanguageChangeable
{
    [SerializeField] private TMP_Text _nameLabel, _wagesInfo, _productivityLabel, _radiusLabel;
    private GeologistController _focusedGeologist;
    [SerializeField] private LanguageItem workers, radius, wages;
    public override void Initialize(GeologistInfoTileAction action, GeologistController tileController)
    {
        _focusedGeologist = tileController;
        UpdateText();
        if (_focusedGeologist != null)
            _focusedGeologist.GetComponent<BuildingRangeShower>().ShowRadius();
    }

    public void UpdateText()
    {
        _nameLabel.text = _focusedGeologist.config.buildingName.ToString();
        _productivityLabel.text = $"{workers}<color=#594331>{_focusedGeologist.WorkersAmount}";
        _radiusLabel.text = $"{radius}: <color=#594331>{_focusedGeologist.Range}";
        WriteWagesInfo(_focusedGeologist);
    }

    private void WriteWagesInfo(GeologistController tileController)
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
    public override void EndFocus()
    {
        if (_focusedGeologist != null)
            _focusedGeologist.GetComponent<BuildingRangeShower>().HideRadius();
    }
}
