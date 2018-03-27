using System;
using System.Linq;
using StructureMap;

namespace IgniteCQRS
{
    public class Dispatcher : IDispatcher
    {
        private readonly IContainer _container;

        public Dispatcher(IContainer container)
        {
            _container = container;
        }

        public CommandResult Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            ICommandHandler<TCommand> resolvedHandler = null;
            var allResolvedHandlers = _container.GetAllInstances<ICommandHandler<TCommand>>();

            var commandHandlers = allResolvedHandlers as ICommandHandler<TCommand>[] ?? allResolvedHandlers.ToArray();
            if (!commandHandlers.Any())
            {
                throw new Exception($"Unable to resolve a handler for type {command.GetType()}.");
            }
            else
            {
                if (commandHandlers.Count() > 1)
                {
                    throw new Exception($"Ambiguous call to handler for type {command.GetType()}.  There are more than one handler registered to the same type.");
                }
                else
                {
                    resolvedHandler = commandHandlers.First();
                }
            }

            return resolvedHandler?.Execute(command);
        }

        public void Publish<TEvent>(TEvent e) where TEvent : IEvent
        {
            var resolvedEventHandlers = _container.GetAllInstances<IEventHandler<TEvent>>();
            foreach (var handler in resolvedEventHandlers)
            {
                Console.WriteLine("Event notified to type:{0}", handler.GetType());
                handler.Handle(e);
            }
        }

        public TResult ExecuteQuery<TQuery, TResult>(TQuery query)
            where TResult : IQueryResult
            where TQuery : IQuery
        {
            IQueryHandler<TQuery, TResult> handler = null;
            var resolvedHandlers = _container.GetAllInstances<IQueryHandler<TQuery, TResult>>();

            var queryHandlers = resolvedHandlers as IQueryHandler<TQuery, TResult>[] ?? resolvedHandlers.ToArray();
            if (!queryHandlers.Any())
            {
                throw new Exception($"Unable to resolve a query handler for type {query.GetType()}.");
            }

            if (queryHandlers.Count() > 1)
            {
                throw new Exception($"Ambiguous call to handler for type {query.GetType()}.  There are more than one handler registered to the same type.");
            }

            handler = queryHandlers.First();
            return handler.ExecuteQuery(query);
        }
    }
}
