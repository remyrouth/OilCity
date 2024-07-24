using System;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace Game.Managers
{
    public class SettingsManager : Singleton<SettingsManager>
    {
        public Language CurrentLanguage { get; private set; } = Language.English;
        
        public delegate void VolumeChanged(float newVolume);
        public event VolumeChanged OnSoundEffectVolumeChanged;
        public event VolumeChanged OnAmbientSoundVolumeChanged;
        public event VolumeChanged OnMusicVolumeChanged;

        private float soundEffectVolume;
        private float ambientSoundVolume;
        private float musicVolume;

        private void Awake()
        {
            SetLanguage();
        }

        public void SetLanguage(Language newLanguage)
        {
            CurrentLanguage = newLanguage;
        
            var languageBasedObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .OfType<ILanguageChangeable>();
            foreach (var lbo in languageBasedObjects)
                lbo.UpdateText();
        
            PlayerPrefs.SetString("Language", CurrentLanguage.ToString());
        }

        public void SetLanguage()
        {
            if (!PlayerPrefs.HasKey("Language"))
            {
                return;
            }
            CurrentLanguage = PlayerPrefs.GetString("Language") switch
            {
                "English" => Language.English,
                "Polish" => Language.Polish,
                _ => Language.English
            };
            SetLanguage(CurrentLanguage);
        }

        public float SoundEffectVolume
        {
            get { return soundEffectVolume; }
            set
            {
                if (soundEffectVolume != value)
                {
                    soundEffectVolume = value;
                    OnSoundEffectVolumeChanged?.Invoke(soundEffectVolume);
                }
            }
        }

        public float AmbientSoundVolume
        {
            get { return ambientSoundVolume; }
            set
            {
                if (ambientSoundVolume != value)
                {
                    ambientSoundVolume = value;
                    OnAmbientSoundVolumeChanged?.Invoke(ambientSoundVolume);
                }
            }
        }

        public float MusicVolume
        {
            get { return musicVolume; }
            set
            {
                if (musicVolume != value)
                {
                    musicVolume = value;
                    OnMusicVolumeChanged?.Invoke(musicVolume);
                }
            }
        }
    
    }

}
