using Game.Managers;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace UIScripts.Settings
{
    public class SfxVolumeChanger : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI valueLabel;
    
        // public void OnEnable()
        // {
        //     if (PlayerPrefs.HasKey("SoundEffectVolume"))
        //     {
        //         var prefValue = PlayerPrefs.GetFloat("SoundEffectVolume");
        //         slider.value = prefValue;
        //         var intValue = (int)(prefValue * 100);
        //         valueLabel.text = intValue.ToString();
        //     }
        // }

        public void SetSfxVolume(float value)
        {
            SettingsManager.Instance.SoundEffectVolume = value;
            var intValue = (int)(value * 100);
            valueLabel.text = intValue.ToString();
        }
    }
}
