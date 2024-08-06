public class NewPipeHelper
{
    public static Relation Flip(Relation relation)
    {
        if (relation == Relation.Output) return Relation.Input;
        if (relation == Relation.Input) return Relation.Output;

        return relation;
    }
}
