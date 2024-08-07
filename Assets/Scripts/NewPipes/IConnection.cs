public interface IConnection
{
    void EstablishConnection(Game.New.IFlowable f, Relation r);

    bool IsValidConnection(Game.New.IFlowable f, Relation r);

    void Remove();

    void EvaluateConnections();

    void ClearRelation(Game.New.IFlowable f);

    void SetRelation(Game.New.IFlowable f, Relation r);

    RelationCollection GetRelations();
}
