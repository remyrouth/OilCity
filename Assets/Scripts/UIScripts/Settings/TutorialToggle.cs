using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;

namespace UIScripts.Settings
{
    public class TutorialToggle : MonoBehaviour
    {
        [SerializeField] private Toggle tutorialToggle;
    
        public void OnEnable()
        {
            if (PlayerPrefs.HasKey("TutorialEnabled"))
            {
                tutorialToggle.isOn = PlayerPrefs.GetInt("TutorialEnabled") == 1;
            }
        }
    
        public void ToggleTutorial()
        {
            TutorialManager.Instance.TutorialEnabled = tutorialToggle.isOn;
            PlayerPrefs.SetInt("TutorialEnabled", TutorialManager.Instance.TutorialEnabled ? 1 : 0);
        }
    }
}
