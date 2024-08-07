using Game.Events;

namespace Game.Tutorial
{
    public class TutorialPaymentIncreaseStep : TutorialStep,ITickReceiver
    {
        public override void Initialize()
        {
            BuildingEvents.OnLumberjackPaymentIncrease += FinishStep;
            base.Initialize();
            TimeManager.Instance.RegisterReceiver(this);
            TimeManager.Instance.TicksPerMinute = 60;
            TileSelector.Instance.SelectorEnabled = true;
        }

        public override void Deinitialize()
        {
            BuildingEvents.OnLumberjackPaymentIncrease -= FinishStep;
            TimeManager.Instance.TicksPerMinute = 0;
            TimeManager.Instance.DeregisterReceiver(this);
        }

        private new void FinishStep()
        {
            TileSelector.Instance.SelectorEnabled = false;
            base.FinishStep();
        }

        private int _timer = 0;
        public void OnTick()
        {
            _timer++;
            if (_timer > 4)
            {
                TimeManager.Instance.TicksPerMinute = 0;
            }
        }
    }
}
