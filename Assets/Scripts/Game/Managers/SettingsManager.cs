using UnityEngine;
using System.Linq;

namespace Game.Managers
{
    public class SettingsManager : Singleton<SettingsManager>
    {
        public Language CurrentLanguage { get; private set; } = Language.English;
        
        public delegate void VolumeChanged(float newVolume);
        public event VolumeChanged OnSoundEffectVolumeChanged;
        public event VolumeChanged OnAmbientSoundVolumeChanged;
        public event VolumeChanged OnMusicVolumeChanged;

        private float soundEffectVolume = 1f;
        private float ambientSoundVolume = 1f;
        private float musicVolume = 1f;

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
        
        public void ToggleCameraMovementInversion()
        {
            CameraController.Instance.invert = !CameraController.Instance.invert;
            PlayerPrefs.SetInt("CameraInversion", CameraController.Instance.invert ? 1 : 0);
        }

        public float SoundEffectVolume
        {
            get { return soundEffectVolume; }
            set
            {
                if (soundEffectVolume != value)
                {
                    soundEffectVolume = value;
                    PlayerPrefs.SetFloat("SFXSoundVolume", soundEffectVolume);
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
                    PlayerPrefs.SetFloat("AmbientSoundVolume", ambientSoundVolume);
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
                    PlayerPrefs.SetFloat("MusicVolume", musicVolume);
                    OnMusicVolumeChanged?.Invoke(musicVolume);
                }
            }
        }
    }
}
