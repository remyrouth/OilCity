using TMPro;
using UnityEngine;

public class CinemaInfoView : BuildingInfoView<CinemaInfoTileAction, EntertainmentBuilding>,ILanguageChangeable
{
    [SerializeField] private TMP_Text _nameLabel, _wagesInfo, _productivityLabel;
    private EntertainmentBuilding _focusedCinema;
    [SerializeField] private LanguageItem productivity, wages;
    public override void Initialize(CinemaInfoTileAction action, EntertainmentBuilding tileController)
    {
        _focusedCinema = tileController;
        UpdateText();
    }

    public void UpdateText()
    {
        _nameLabel.text = _focusedCinema.config.buildingName.ToString();
        _productivityLabel.text = $"{productivity}: {_focusedCinema.AmountPerTick()}";
        WriteWagesInfo(_focusedCinema);
    }

    private void WriteWagesInfo(EntertainmentBuilding tileController)
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
