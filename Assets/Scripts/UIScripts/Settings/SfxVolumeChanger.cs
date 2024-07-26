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
            if (PlayerPrefs.HasKey("SFXSoundVolume"))
            {
                sfxSlider.value = PlayerPrefs.GetFloat("SFXSoundVolume");
            }
        }

        public void SetSfxVolume(float value)
        {
            SettingsManager.Instance.SoundEffectVolume = value;
        }
    }
}
