using Game.Managers;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace UIScripts.Settings
{
    public class MusicVolumeChanger : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI valueLabel;
        
        public void OnEnable()
        {
            var prefValue = SettingsManager.Instance.MusicVolume;
            slider.value = prefValue;
            var intValue = (int)(prefValue * 100);
            valueLabel.text = intValue.ToString();
        }

        public void SetMusicVolume(float value)
        {
            SettingsManager.Instance.MusicVolume = value;
            var intValue = (int)(value * 100);
            valueLabel.text = intValue.ToString();
        }
    }
}
