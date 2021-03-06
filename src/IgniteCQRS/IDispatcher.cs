﻿using System.Threading.Tasks;

namespace IgniteCQRS
{
    public interface IDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<CommandResult> ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand;

        /// <summary>
        /// Notifies an event to all suscribers
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="e"></param>
        Task PublishAsync<TEvent>(TEvent e) where TEvent : IEvent;
        
        /// <summary>
        /// Given a query, it resolves to the respective query handler and executes the handle method to return the result.
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TResult> ExecuteQueryAsync<TQuery, TResult>(TQuery query)
            where TResult : IQueryResult
            where TQuery : IQuery;
    }
}
