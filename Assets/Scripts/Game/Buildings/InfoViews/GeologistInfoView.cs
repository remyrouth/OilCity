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
        _productivityLabel.text = $"{_focusedGeologist.WorkersAmount} {workers}";
        _radiusLabel.text = $"{radius}: {_focusedGeologist.Range}";
        WriteWagesInfo(_focusedGeologist);
    }

    private void WriteWagesInfo(GeologistController tileController)
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
    public override void EndFocus()
    {
        if (_focusedGeologist != null)
            _focusedGeologist.GetComponent<BuildingRangeShower>().HideRadius();
    }
}
