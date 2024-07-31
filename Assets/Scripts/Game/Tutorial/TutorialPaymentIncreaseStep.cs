using Game.Events;

namespace Game.Tutorial
{
    public class TutorialPaymentIncreaseStep : TutorialStep
    {
        private new void OnEnable()
        {
            BuildingEvents.OnLumberjackPaymentIncrease += FinishStep;
            base.OnEnable();
            TimeManager.Instance.TicksPerMinute = 60;
            TileSelector.Instance.SelectorEnabled = true;
        }

        private void OnDisable()
        {
            BuildingEvents.OnLumberjackPaymentIncrease -= FinishStep;
        }

        private new void FinishStep()
        {
            TileSelector.Instance.SelectorEnabled = false;
            base.FinishStep();
        }
    }
}
