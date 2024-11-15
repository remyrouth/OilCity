using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Game.Managers
{
    public class SettingsManager : Singleton<SettingsManager>
    {
        public Language CurrentLanguage { get; private set; } = Language.English;
        
        public delegate void VolumeChanged(float newVolume);
        public event VolumeChanged OnSoundEffectVolumeChanged;
        public event VolumeChanged OnAmbientSoundVolumeChanged;
        public event VolumeChanged OnMusicVolumeChanged;

        private float _masterVolume = 1f;
        private float _soundEffectVolume = 1f;
        private float _ambientSoundVolume = 1f;
        private float _musicVolume = 1f;

        [SerializeField]
        private VolumePreferences preferences;
        
        [SerializeField]
        private bool willRestartPrefsOnLoad = false;

        public void Update() {

        }
        
        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(this);
            }
            
            DontDestroyOnLoad(this);
            
            SetLanguage();
            SetCameraMovementInversion();

        }

        
        public void SetLanguage(Language newLanguage)
        {
            CurrentLanguage = newLanguage;
        
            var languageBasedObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .OfType<ILanguageChangeable>();
            foreach (var lbo in languageBasedObjects)
                lbo.UpdateText();

            if (!SceneManager.GetActiveScene().name.Equals("MainMenu") && DialogueUI.Instance.CurrentDialogue is not null)
            {
                DialogueUI.Instance.ChangeTextLanguage();
            }

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
        
        private void SetCameraMovementInversion()
        {

            if (PlayerPrefs.HasKey("CameraInversion"))
            {
                CameraController.Instance.Invert = PlayerPrefs.GetInt("CameraInversion") == 1;
            }
        }

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = value;
                PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
                OnSoundEffectVolumeChanged?.Invoke(_masterVolume * _soundEffectVolume);
                OnAmbientSoundVolumeChanged?.Invoke(_masterVolume * _ambientSoundVolume);
                OnMusicVolumeChanged?.Invoke(_masterVolume * _musicVolume);
            } 
        }

        public float SoundEffectVolume
        {
            get => _soundEffectVolume;
            set
            {
                if (_soundEffectVolume == value)
                {
                    return;
                }
                _soundEffectVolume = value;
                PlayerPrefs.SetFloat("SoundEffectVolume", _soundEffectVolume);
                OnSoundEffectVolumeChanged?.Invoke(_masterVolume * _soundEffectVolume);
            }
        }

        public float AmbientSoundVolume
        {
            get => _ambientSoundVolume; 
            set
            {
                if (_ambientSoundVolume == value)
                {
                    return;
                }
                _ambientSoundVolume = value;
                PlayerPrefs.SetFloat("AmbientSoundVolume", _ambientSoundVolume);
                OnAmbientSoundVolumeChanged?.Invoke(_masterVolume * _ambientSoundVolume);
            }
        }

        public float MusicVolume
        {
            get => _musicVolume; 
            set
            {
                if (_musicVolume == value)
                {
                    return;
                }
                _musicVolume = value;
                PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
                OnMusicVolumeChanged?.Invoke(_masterVolume * _musicVolume);
            }
        }
    }
}
