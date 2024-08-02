using TMPro;
using UnityEngine;

public class CinemaInfoView : BuildingInfoView<CinemaInfoTileAction, EntertainmentBuilding>
{
    [SerializeField] private TMP_Text _nameLabel, _descriptionLabel, _productivityLabel, _wageLabel;
    private EntertainmentBuilding _focusedCinema;
    public override void Initialize(CinemaInfoTileAction action, EntertainmentBuilding tileController)
    {
        _focusedCinema = tileController;
        _productivityLabel.text = $"Worker satisfaction increased by: {tileController.CurrentSatisfactionIncreaseValue}";
        _nameLabel.text = tileController.config.buildingName.ToString();
        _descriptionLabel.text = tileController.config.description.ToString();
        _wageLabel.text = tileController.CurrentPaymentMode.ToString();
    }
    
}
