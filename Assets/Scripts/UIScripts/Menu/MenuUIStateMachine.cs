using System.Linq;
using UnityEngine;

public class MenuUIStateMachine : Singleton<MenuUIStateMachine>
{
    [SerializeField] private MenuUIState[] states;

    private MenuUIState Current;

    private void Start()
    {
        ChangeState(MenuUIType.Main);
    }

    public void ChangeState(MenuUIType type)
    {
        if (Current != null)
            Current.OnExit();
        Current = states.First(e => e.type == type);
        Current.OnEnter();
    }
    private void Update()
    {
        if (Current != null)
            Current.OnUpdate();
    }

    public enum MenuUIType { Main, Settings, Credits, DifficultyChoose }
}
