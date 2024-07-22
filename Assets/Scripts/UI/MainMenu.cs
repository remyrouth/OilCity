using UnityEngine;

namespace UI
{
    public class MainMenu : Singleton<MainMenu>
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditsPanel;
        
        public void StartGame()
        {
            gameObject.SetActive(false);
        }
        
        public void GoToSettings()
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }
        
        public void GoToCredits()
        {
            mainPanel.SetActive(false);
            creditsPanel.SetActive(true);
        }
        
        public void BackToMainPanel()
        {
            mainPanel.SetActive(true);
            settingsPanel.SetActive(false);
            creditsPanel.SetActive(false);
        }
        
        public void ExitGame()
        {
            Application.Quit();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
