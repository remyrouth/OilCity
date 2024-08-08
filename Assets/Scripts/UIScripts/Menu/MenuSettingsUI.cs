using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSettingsUI : MenuUIState
{
    public override MenuUIStateMachine.MenuUIType type => MenuUIStateMachine.MenuUIType.Settings;
    public override void OnEnter()
    {
        base.OnEnter();
        parallax.overrideMouseX = null;
    }
}
