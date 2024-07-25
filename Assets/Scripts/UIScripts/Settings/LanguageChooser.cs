using Game.Managers;
using TMPro;
using UnityEngine;

namespace UIScripts.Settings
{
    public class LanguageChooser : MonoBehaviour
    {

        private TMP_Dropdown _languageDropdown;
    
        public void OnEnable()
        {
            _languageDropdown = GetComponent<TMP_Dropdown>();
            _languageDropdown.SetValueWithoutNotify((int)SettingsManager.Instance.CurrentLanguage);
        }
    
        public void SetLanguage(int languageIndex)
        {
            SettingsManager.Instance.SetLanguage((Language)languageIndex);
        }
    }
 
}

