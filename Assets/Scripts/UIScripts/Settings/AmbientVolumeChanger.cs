using Game.Managers;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;


namespace UIScripts.Settings
{
    public class AmbientVolumeChanger : MonoBehaviour
    {
        [SerializeField] private Slider ambientSlider;

        public void OnEnable()
        {
            if (PlayerPrefs.HasKey("AmbientSoundVolume"))
            {
                ambientSlider.value = PlayerPrefs.GetFloat("AmbientSoundVolume");
            }
        }

        public void SetAmbientVolume(float value)
        {
            SettingsManager.Instance.AmbientSoundVolume = value;
        }
    }

}
