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
        [SerializeField] private TMP_Dropdown languageDropdown;

        private void Awake()
        {
            SetLanguage();
        }

        public void SetLanguage(int languageIndex)
        {
            CurrentLanguage = languageIndex switch
            {
                0 => Language.English,
                1 => Language.Polish,
                _ => Language.English
            };
        
            var languageBasedObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .OfType<ILanguageChangeable>();
            foreach (var lbo in languageBasedObjects)
                lbo.UpdateText();
        
            PlayerPrefs.SetString("Language", CurrentLanguage.ToString());
            PlayerPrefs.SetInt("LanguageIndex", languageDropdown.value);
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
        
            var languageBasedObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .OfType<ILanguageChangeable>();
            foreach (var lbo in languageBasedObjects)
                lbo.UpdateText();
            
            languageDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("LanguageIndex"));
        }
    
    }

}
