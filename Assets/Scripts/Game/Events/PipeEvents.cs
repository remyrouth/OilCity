using System;

namespace Game.Events
{
    public static class PipeEvents
    {
        public static event Action OnPipePlaced;
        public static void PlacePipe()
        {
            OnPipePlaced?.Invoke();
        }
    }
}