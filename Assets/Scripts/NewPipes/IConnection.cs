public interface IConnection
{
    void EstablishConnection(Game.New.IFlowable f, Relation r);

    bool IsValidConnection((bool can_input, bool can_output) io, (FlowType in_type, FlowType out_type) fc, Relation r);

    void Remove();

    void EvaluateConnections();

    void ClearRelation(Game.New.IFlowable f);

    void SetRelation(Game.New.IFlowable f, Relation r);
}
