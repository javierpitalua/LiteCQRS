using System.Threading.Tasks;

namespace IgniteCQRS
{
    public interface IQuery
    {

    }

    public interface IQueryResult
    {

    }

    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery
        where TResult : IQueryResult
    {
        Task<TResult> ExecuteQueryAsync(TQuery query);
    }
}
