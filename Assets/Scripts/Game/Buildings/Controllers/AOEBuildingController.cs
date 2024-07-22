public abstract class AOEBuildingController : PayrateBuildingController
{
    public abstract int TickNumberInterval { get; }
    public abstract int Range { get; }
    protected abstract void InvokeAction();
}
