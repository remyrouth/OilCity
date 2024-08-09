using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStateMachine : Singleton<UIStateMachine>
{
    private Dictionary<GameState, UIState> _states;
    private GameState _previousState;

    [SerializeField] private GameState initialGameState;
    private Dictionary<GameState, UIState> States
    {
        get
        {
            if (_states == null)
            {
                _states = new();
                foreach (var state in FindObjectsOfType<UIState>())
                    _states.Add(state.type, state);
            }
            return _states;
        }
    }
    private void Awake()
    {
        ChangeState(initialGameState);
    }
    public UIState CurrentState { get; private set; }
    public GameState CurrentStateType => CurrentState.type;

    /// <summary>
    /// Changes UI state and calls OnExit and OnEntered
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(GameState state)
    {
        Debug.Log(state);
        if (!States.ContainsKey(state))
        {
            Debug.LogError("Tried to change UI state to non-existing one!");
            return;
        }
        if (CurrentState != null){
            _previousState = CurrentState.type;
            CurrentState.OnExit();
        }
        CurrentState = States[state];
        CurrentState.OnEnter();
        Debug.Log(CurrentStateType);
    }

    /// <summary>
    /// Changes game Scene
    /// </summary>
    /// <param name="sceneIndex"></param>
    public void ReturnToPreviousState()
    {
        ChangeState(_previousState);
    }
    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    private void Update()
    {
        if (CurrentState != null)
            CurrentState.OnUpdate();
    }


}
public enum GameState { PauseUI, GameUI, BuildingUI, EventUI, EndingUI, ChoosingTutorialUI }
