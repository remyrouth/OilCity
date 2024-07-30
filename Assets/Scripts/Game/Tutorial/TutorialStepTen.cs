using Game.Tutorial;
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
