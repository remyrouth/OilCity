using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MenuUIState
{
    public override MenuUIStateMachine.MenuUIType type => MenuUIStateMachine.MenuUIType.Main;
    public override void OnEnter()
    {
        base.OnEnter();
        parallax.overrideMouseX = null;
    }
}
