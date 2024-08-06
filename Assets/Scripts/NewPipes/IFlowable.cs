namespace Game.New
{
    public interface IFlowable : IConnection
    {
        (bool can_input, bool can_output) GetIOConfig();


    }

}