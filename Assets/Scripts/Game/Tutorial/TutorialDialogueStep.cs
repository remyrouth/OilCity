using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialDialogueStep : TutorialStep
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
}
