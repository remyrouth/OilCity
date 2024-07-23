using UIScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem
{
    public class InputManager : MonoBehaviour, PlayerControlls.IUIActions
    {
        private PlayerControlls _controlls;
        private bool _gamePaused;

        private void OnDisable()
        {
            _controlls.UI.RemoveCallbacks(this);
        }
        
        private void Awake()
        {
            PrepareInputSystem();
        }

        private void PrepareInputSystem()
        {
            _controlls = new PlayerControlls();
            _gamePaused = false;
            
            _controlls.UI.SetCallbacks(this);
            _controlls.UI.Enable();

            Time.timeScale = 1;
        }
        
        public void OnTogglePauseMenu()
        {
            if (_gamePaused)
            {
                UiManager.Instance.ChangeUI(0);
                Time.timeScale = 1;
            }
            else
            {
                UiManager.Instance.ChangeUI(1);
                Time.timeScale = 0;
            }
            _gamePaused = !_gamePaused;
        }
        
        public void OnTogglePauseMenu(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnTogglePauseMenu();
            }
        }
    }
}