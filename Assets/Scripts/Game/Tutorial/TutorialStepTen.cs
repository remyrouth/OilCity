using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStepTen : TutorialStep
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