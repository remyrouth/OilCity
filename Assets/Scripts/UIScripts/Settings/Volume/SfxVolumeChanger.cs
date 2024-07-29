using Game.Managers;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace UIScripts.Settings
{
    public class SfxVolumeChanger : MonoBehaviour
    {
        [SerializeField] private Slider sfxSlider;
    
        public void OnEnable()
        {
            if (PlayerPrefs.HasKey("SoundEffectVolume"))
            {
                sfxSlider.value = PlayerPrefs.GetFloat("SoundEffectVolume");
            }
        }

        public void SetSfxVolume(float value)
        {
            SettingsManager.Instance.SoundEffectVolume = value;
        }
    }
}
