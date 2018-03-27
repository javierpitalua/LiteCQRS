namespace IgniteCQRS
{
    public interface IQuery
    {

    }

    public interface IQueryResult
    {

    }

    public interface IQueryHandler<in TQuery, out TResult>
        where TQuery : IQuery
        where TResult : IQueryResult
    {
        TResult ExecuteQuery(TQuery query);
    }
}
