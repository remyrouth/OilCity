using Game.Managers;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class MasterVolumeChanger : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    public void OnEnable()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }
    }

    public void SetMasterVolume(float value)
    {
        SettingsManager.Instance.MasterVolume = value;
    }
}
