/// <summary>
/// An interface for classes that are meant to receive game ticks.
/// </summary>
public interface ITickReceiver
{
    /// <summary>
    /// The method to be invoked on every game tick.
    /// </summary>
    void OnTick();
}
