using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialLastStep : TutorialStep
    {
        public override void Initialize()
        {
            DialogueUI.Instance.OnDialogueClicked += FinishStep;
            DialogueUI.Instance.ToggleIndicator();
            base.Initialize();
        }

        public override void Deinitialize()
        {
            DialogueUI.Instance.OnDialogueClicked -= FinishStep;
        }
    
        private new void FinishStep()
        {
            DialogueUI.Instance.ToggleIndicator();
            DialogueUI.Instance.DisableDialogue();
            BuildingPanelUI.Instance.EnableAllButtons();
            TimeManager.Instance.TicksPerMinute = 60;
            TileSelector.Instance.SelectorEnabled = true;
            TutorialManager.Instance.InTutorial = false;

            Destroy(gameObject);
        }
    }

}
