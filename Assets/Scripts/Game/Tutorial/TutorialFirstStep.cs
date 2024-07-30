using UnityEngine;

public class TutorialFirstStep : TutorialStep
{
    private new void OnEnable()
    {
        base.OnEnable();
        TimeManager.Instance.TicksPerMinute = 0;
        BuildingPanelUI.Instance.DisableAllButtons();
        DialogueUI.Instance.EnableDialogue();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FinishStep();
        }
    }
}
