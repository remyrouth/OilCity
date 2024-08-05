using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : Singleton<GameStateManager>
{
    [SerializeField]
    private string mainMenuSceneName = "MainMenu";
    public event Action OnGameEnded;
    public void EndGame() {
        //LoadScene(mainMenuSceneName);
        OnGameEnded?.Invoke();
        UIStateMachine.Instance.ChangeState(GameState.EndingUI);
    }
    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
