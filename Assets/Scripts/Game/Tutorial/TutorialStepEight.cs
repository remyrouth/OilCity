using System.Collections;
using System.Collections.Generic;
using Game.Tutorial;
using UnityEngine;

public class TutorialStepEight : TutorialStep
{
    private new void OnEnable()
    {
        base.OnEnable();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FinishStep();
        }
    }
}
