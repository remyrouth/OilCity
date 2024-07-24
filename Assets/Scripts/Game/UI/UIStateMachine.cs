using System.Collections.Generic;
using UnityEngine;

public class UIStateMachine : Singleton<UIStateMachine>
{
    private Dictionary<GameState, UIState> _states;

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
                ChangeState(initialGameState);
            }
            return _states;
        }
    }

    public UIState CurrentState;

    /// <summary>
    /// Changes UI state and calls OnExit and OnEntered
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(GameState state)
    {
        if (!States.ContainsKey(state))
        {
            Debug.LogError("Tried to change UI state to non-existing one!");
            return;
        }
        if (CurrentState != null)
            CurrentState.OnExit();
        CurrentState = States[state];
        CurrentState.OnEnter();
    }
    private void Update()
    {
        if (CurrentState != null)
            CurrentState.OnUpdate();
    }


}
public enum GameState { GameUI, MenuUI, BuildingUI, EventUI, EndingUI }
