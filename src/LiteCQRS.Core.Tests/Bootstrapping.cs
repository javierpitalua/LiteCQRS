using System;
using System.Collections.Generic;
using IgniteCQRS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace LiteCQRS.Core.Tests
{
    [TestClass]
    public class Bootstrapping
    {
        public class GetEmployeesQuery : IQuery
        {
            public string Name { get; set; }
        }

        public class EmployeeItem
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public class EmployeeResult : IQueryResult
        {
            public List<EmployeeItem> Employees { get; set; }
        }

        public class GetEmployeesQueryHandler : IQueryHandler<GetEmployeesQuery, EmployeeResult>
        {
            public EmployeeResult ExecuteQuery(GetEmployeesQuery query)
            {
                return new EmployeeResult()
                {
                    Employees = new List<EmployeeItem>()
                    {
                        new EmployeeItem() { Name = "Pedro", Email = "pedro@now.com" },
                        new EmployeeItem() { Name = "Javier", Email = "javier@yes.com" }
                    }
                };
            }
        }

        public class GreetingsCommand : ICommand, IValidatable
        {
            public string Name { get; set; }

            public ValidationResult Validate()
            {
                return new ValidationResult();
            }
        }

        public class GreetingExecutedEvent : IEvent
        {
            public string Who { get; set; }
            public string What { get; set; }
        }

        public class GreetingsCommandHandler : ICommandHandler<GreetingsCommand>
        {
            private readonly IDispatcher _dispatcher;

            public GreetingsCommandHandler(IDispatcher mediator)
            {
                _dispatcher = mediator;
            }

            public CommandResult Execute(GreetingsCommand command)
            {
                Console.WriteLine("Hola");
                var result = new CommandResult()
                {
                    OperationSuccesful = true
                };
                _dispatcher.Publish(new GreetingExecutedEvent() { What = "Everything is awesome!", Who = "Emmet" });
                return result;
            }
        }

        public class EventHandler1 : IEventHandler<GreetingExecutedEvent>
        {
            public void Handle(GreetingExecutedEvent e)
            {
                Console.WriteLine("Greetings from handler 1: {0} says {1}", e.Who, e.What);
            }
        }

        public class EventHandler2 : IEventHandler<GreetingExecutedEvent>
        {
            public void Handle(GreetingExecutedEvent e)
            {
                Console.WriteLine("Greetings from handler 2: {0} says {1}", e.Who, e.What);
            }
        }

        public interface IValidatable
        {
            ValidationResult Validate();
        }

        public class ValidationDecorator<T> : ICommandHandler<T> where T : ICommand
        {
            private readonly ICommandHandler<T> _inner;

            public ValidationDecorator(ICommandHandler<T> inner)
            {
                _inner = inner;
            }

            public CommandResult Execute(T command)
            {
                if (!(command is IValidatable validatable)) return _inner.Execute(command);
                Console.WriteLine("ValidateFirst");
                validatable.Validate();
                return _inner.Execute(command);
            }
        }

        private readonly Container _container;

        public Bootstrapping()
        {
            _container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AssemblyContainingType<IDispatcher>();
                    x.Assembly("LiteCQRS.Core.Tests");  
                    x.ConnectImplementationsToTypesClosing(typeof(ICommandHandler<>));
                    x.ConnectImplementationsToTypesClosing(typeof(IQueryHandler<,>));
                    x.ConnectImplementationsToTypesClosing(typeof(IEventHandler<>));
                    x.WithDefaultConventions();
                    x.Exclude(type => type == typeof(ValidationDecorator<>));
                });

                _.For(typeof(ICommandHandler<>)).DecorateAllWith(typeof(ValidationDecorator<>));
            });
        }

        [TestMethod]
        public void BootstrappingTest()
        {
            var dispatcher = _container.GetInstance<IDispatcher>();

            var result = dispatcher.Execute<GreetingsCommand>(new GreetingsCommand() { Name = "Hola" });
            var queryResult = dispatcher.ExecuteQuery<GetEmployeesQuery, EmployeeResult>(new GetEmployeesQuery() { Name = "Javier" });

            Assert.IsTrue(result.OperationSuccesful == true);
            Assert.IsTrue(queryResult.Employees.Count > 1);
        }
    }
}
