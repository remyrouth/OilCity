using Game.Events;
using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialPipeStep : TutorialStep
    {
        [SerializeField] private int buttonToUnlockIndex;
        private new void OnEnable()
        {
            PipeEvents.OnPipePlaced += FinishStep;
            base.OnEnable();
            BuildingPanelUI.Instance.ToggleButtonInteractable(buttonToUnlockIndex);
        }
    
        private void OnDisable()
        {
            PipeEvents.OnPipePlaced -= FinishStep;
        }
    
        private new void FinishStep()
        {
            base.FinishStep();
        }
    }

}
