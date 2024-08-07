using Game.Managers;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace UIScripts.Settings
{
    public class AmbientVolumeChanger : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI valueLabel;

        // public void OnEnable()
        // {
        //     if (PlayerPrefs.HasKey("AmbientSoundVolume"))
        //     {
        //         var prefValue = PlayerPrefs.GetFloat("AmbientSoundVolume");
        //         slider.value = prefValue;
        //         var intValue = (int)(prefValue * 100);
        //         valueLabel.text = intValue.ToString();
        //     }
        // }

        public void SetAmbientVolume(float value)
        {
            SettingsManager.Instance.AmbientSoundVolume = value;
            var intValue = (int)(value * 100);
            valueLabel.text = intValue.ToString();
        }
    }

}

