using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using StructureMap;

namespace LiteCQRS
{
    public interface ICommand
    {

    }

    public interface IEvent
    {

    }

    public interface IQuery
    {

    }

    public interface IQueryResult
    {

    }

    public class ValidationFailure
    {
        public string PropertyName { get; set; }
        public string ErrorDescription { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Summary { get; set; }
        public IEnumerable<ValidationFailure> ValidationDetails { get; set; }
    }

    public class CommandResult
    {
        public string OperationId { get; set; }
        public bool OperationSuccesful { get; set; }
        public ValidationResult ValidationResult { get; set; }
        public Dictionary<string, string> ExtendedProperties { get; set; }

        public CommandResult()
        {
            ValidationResult = new ValidationResult();
            ExtendedProperties = new Dictionary<string, string>();
        }
    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        CommandResult Execute(TCommand command);
    }

    public interface IQueryHandler<in TQuery, out TResult>
        where TQuery : IQuery
        where TResult : IQueryResult
    {
        TResult ExecuteQuery(TQuery query);
    }

    public interface IEventHandler<in T> where T : IEvent
    {
        void Handle(T e);
    }

    public interface IMediator
    {
        CommandResult ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand;
        void Publish<TEvent>(TEvent e) where TEvent : IEvent;
        TResult ExecuteQuery<TResult, TQuery>(TQuery query)
            where TResult : IQueryResult
            where TQuery : IQuery;

    }

    public class Mediator : IMediator
    {
        private readonly IContainer _container;

        public Mediator(IContainer container)
        {
            _container = container;
        }

        public CommandResult ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            ICommandHandler<TCommand> handler = null;
            var resolvedHandlers = _container.GetAllInstances<ICommandHandler<TCommand>>();

            var commandHandlers = resolvedHandlers as ICommandHandler<TCommand>[] ?? resolvedHandlers.ToArray();
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
                    handler = commandHandlers.First();
                }
            }

            return handler?.Execute(command);
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

        public TResult ExecuteQuery<TResult, TQuery>(TQuery query)
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
