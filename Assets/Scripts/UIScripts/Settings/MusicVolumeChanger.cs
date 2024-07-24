using Game.Managers;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace UIScripts.Settings
{
    public class MusicVolumeChanger : MonoBehaviour
    {
        [SerializeField] private Slider musicSlider;
        
        public void OnEnable()
        {
            musicSlider.value = SettingsManager.Instance.MusicVolume;
        }

        public void SetMusicVolume(float value)
        {
            SettingsManager.Instance.MusicVolume = value;
        }
    }
}
