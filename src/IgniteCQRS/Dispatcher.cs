using System;
using System.Linq;
using System.Threading.Tasks;
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
        
        public Task<CommandResult> ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
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

            return resolvedHandler?.ExecuteAsync(command);
        }

        public async Task PublishAsync<TEvent>(TEvent e) where TEvent : IEvent
        {
            var resolvedEventHandlers = _container.GetAllInstances<IEventHandler<TEvent>>();
            foreach (var handler in resolvedEventHandlers)
            {
                Console.WriteLine("Event notified to type:{0}", handler.GetType());
                await handler.Handle(e);
            }
        }

        public Task<TResult> ExecuteQueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery where TResult : IQueryResult
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
            return handler.ExecuteQueryAsync(query);
        }
    }
}
