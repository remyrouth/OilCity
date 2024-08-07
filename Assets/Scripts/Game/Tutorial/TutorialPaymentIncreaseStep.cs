using Game.Events;

namespace Game.Tutorial
{
    public class TutorialPaymentIncreaseStep : TutorialStep
    {
        public override void Initialize()
        {
            BuildingEvents.OnLumberjackPaymentIncrease += FinishStep;
            base.Initialize();
            TimeManager.Instance.TicksPerMinute = 60;
            TileSelector.Instance.SelectorEnabled = true;
        }

        public override void Deinitialize()
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
