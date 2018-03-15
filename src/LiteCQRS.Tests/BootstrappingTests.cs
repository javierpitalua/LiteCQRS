using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace LiteCQRS.Tests
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

    public class GreetingsCommand : LiteCQRS.ICommand
    {
        public string Name { get; set; }
    }

    public class GreetingExecutedEvent : IEvent
    {
        public string Who { get; set; }
        public string What { get; set; }
    }

    public class GreetingsCommandHandler : LiteCQRS.ICommandHandler<GreetingsCommand>
    {
        private IMediator _mediator;

        public GreetingsCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public CommandResult Execute(GreetingsCommand command)
        {
            Console.WriteLine("Hola");
            var result = new CommandResult()
            {
                OperationSuccesful = true
            };
            _mediator.Publish(new GreetingExecutedEvent() { What = "Everything is awesome!", Who = "Emmet" });
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

    [TestClass]
    public class BootstrappingTests
    {
        [TestMethod]
        public void InvocationTest()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AssemblyContainingType<LiteCQRS.IMediator>();
                    x.ConnectImplementationsToTypesClosing(typeof(LiteCQRS.ICommandHandler<>));
                    x.ConnectImplementationsToTypesClosing(typeof(LiteCQRS.IQueryHandler<,>));
                    x.ConnectImplementationsToTypesClosing(typeof(LiteCQRS.IEventHandler<>));
                    x.WithDefaultConventions();
                });
            });

            var mediator = container.GetInstance<IMediator>();

            var result = mediator.ExecuteCommand<GreetingsCommand>(new GreetingsCommand() { Name = "Hola" });
            var queryResult = mediator.ExecuteQuery<EmployeeResult, GetEmployeesQuery>(new GetEmployeesQuery() {Name = "Javier"});

            Assert.IsTrue(result.OperationSuccesful == true);
        }
    }
}
