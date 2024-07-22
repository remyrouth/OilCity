public sealed class WoodCutterController : AOEBuildingController
{
    public override int TickNumberInterval => 1;

    public override int Range => 1;

    protected override void InvokeAction()
    {
        //destroy a tree
        throw new System.NotImplementedException();
    }
}
