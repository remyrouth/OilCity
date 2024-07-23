using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIScripts
{
    public class UiManager : Singleton<UiManager>
    {
        [SerializeField] private GameObject[] availableUserInterfaces;
        private GameObject _currentUI;

        private void Awake()
        {
            _currentUI = availableUserInterfaces[0];
        }
        
        public void ChangeScene(int sceneNumber)
        {
            SceneManager.LoadScene(sceneNumber);
        }
        
        public void ChangeUI(int uiIndex)
        {
            _currentUI.SetActive(false);
            _currentUI = availableUserInterfaces[uiIndex];
            _currentUI.SetActive(true);
        }

        public void ChangeVisibility(int uiIndex)
        {
            availableUserInterfaces[uiIndex].SetActive(!availableUserInterfaces[uiIndex].activeSelf);
        }
    }
}