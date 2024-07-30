using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialFirstStep : TutorialStep
    {
        private new void OnEnable()
        {
            base.OnEnable();
            TimeManager.Instance.TicksPerMinute = 0;
            TileSelector.Instance.SelectorEnabled = false;
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
 
}
