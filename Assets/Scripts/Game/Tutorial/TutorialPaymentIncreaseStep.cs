using Game.Events;

namespace Game.Tutorial
{
    public class TutorialPaymentIncreaseStep : TutorialStep
    {
        private new void OnEnable()
        {
            BuildingEvents.OnLumberjackPaymentIncrease += FinishStep;
            base.OnEnable();
            TileSelector.Instance.Enabled = true;
        }

        private void OnDisable()
        {
            BuildingEvents.OnLumberjackPaymentIncrease -= FinishStep;
        }

        private new void FinishStep()
        {
            TileSelector.Instance.Enabled = false;
            base.FinishStep();
        }
    }
}
