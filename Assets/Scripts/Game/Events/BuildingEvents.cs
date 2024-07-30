using System;

namespace Game.Events
{
    public static class BuildingEvents
    {
        public static event Action OnLumberjackPaymentIncrease;
        public static void IncreasePayment()
        {
            OnLumberjackPaymentIncrease?.Invoke();
        }
    }
}
