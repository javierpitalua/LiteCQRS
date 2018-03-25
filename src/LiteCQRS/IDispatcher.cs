using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCQRS
{
    public interface IDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        CommandResult Execute<TCommand>(TCommand command) where TCommand : ICommand;

        /// <summary>
        /// Notifies an event to all suscribers
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="e"></param>
        void Publish<TEvent>(TEvent e) where TEvent : IEvent;

        /// <summary>
        /// Given a query, it resolves to the respective query handler and executes the handle method to return the result.
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        TResult ExecuteQuery<TQuery, TResult>(TQuery query)
            where TResult : IQueryResult
            where TQuery : IQuery;

    }
}
