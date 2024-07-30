using Game.Tutorial;
using UnityEngine;

public class TutorialStepFive : TutorialStep
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FinishStep();
        }
    }
}
