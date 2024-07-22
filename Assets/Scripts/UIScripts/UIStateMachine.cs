using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIScripts
{
    public class UIStateMachine : Singleton<UIStateMachine>
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
        
        public void ChangeState(int stateIndex)
        {
            _currentUI.SetActive(false);
            _currentUI = availableUserInterfaces[stateIndex];
            _currentUI.SetActive(true);
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