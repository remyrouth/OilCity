using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStepEleven : TutorialStep
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
    
    private new void FinishStep()
    {
        DialogueUI.Instance.DisableDialogue();
        TimeManager.Instance.TicksPerMinute = 60;
        Destroy(gameObject);
    }
}
