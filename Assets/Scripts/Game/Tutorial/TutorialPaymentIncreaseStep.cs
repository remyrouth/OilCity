using Game.Events;

namespace Game.Tutorial
{
    public class TutorialPaymentIncreaseStep : TutorialStep
    {
        private new void OnEnable()
        {
            BuildingEvents.OnLumberjackPaymentIncrease += FinishStep;
            base.OnEnable();
        }

        private void OnDisable()
        {
            BuildingEvents.OnLumberjackPaymentIncrease -= FinishStep;
        }

        private new void FinishStep()
        {
            base.FinishStep();
        }
    }
}
