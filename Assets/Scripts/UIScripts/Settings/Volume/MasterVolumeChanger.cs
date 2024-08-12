using Game.Managers;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class MasterVolumeChanger : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueLabel;
    
    public void OnEnable()
    {
        var prefValue = SettingsManager.Instance.MasterVolume;
        slider.value = prefValue;
        var intValue = (int)(prefValue * 100);
        valueLabel.text = intValue.ToString();
    }

    public void SetMasterVolume(float value)
    {
        SettingsManager.Instance.MasterVolume = value;
        var intValue = (int)(value * 100);
        valueLabel.text = intValue.ToString();
    }

    private void Start() {
        SettingsManager.Instance.MasterVolume = slider.value;
    }
}
