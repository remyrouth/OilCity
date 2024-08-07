namespace Game.New
{
    public interface IFlowable : IConnection, ITickReceiver
    {
        (bool can_input, bool can_output) GetIOConfig();

        (FlowType input, FlowType output) GetFlowConfig();
    }

}