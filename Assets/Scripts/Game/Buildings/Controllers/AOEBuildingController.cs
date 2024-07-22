public abstract class AOEBuildingController : PayrateBuildingController,ITickReceiver
{
    public abstract int TickNumberInterval { get; }
    public abstract int Range { get; }

    private int _tickTimer = 0;
    public void OnTick()
    {
        _tickTimer++;
        if (_tickTimer > TickNumberInterval)
        {
            _tickTimer = 0;
            InvokeAction();
        }
    }
    /// <summary>
    /// Called repeatedly after 'TickNumberInterval' number of ticks
    /// </summary>
    protected abstract void InvokeAction();
}
