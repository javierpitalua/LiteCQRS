using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCQRS
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
