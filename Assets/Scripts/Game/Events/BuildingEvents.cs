using System;

namespace Game.Events
{
    public static class BuildingEvents
    {
        public static event Action OnLumberjackPaymentIncrease;
        public static event Action OnCivilianBuildingSpawn;
        public static void IncreasePayment()
        {
            OnLumberjackPaymentIncrease?.Invoke();
        }

        public static void OnCivilianSpawn()
        {
            OnCivilianBuildingSpawn?.Invoke();
        }
    }
}
