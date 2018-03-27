using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace LiteCQRS.Tests
{
    

    [TestClass]
    public class BootstrappingTests
    {
        private readonly IContainer _container;
        public BootstrappingTests()
        {
            _container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AssemblyContainingType<LiteCQRS.IDispatcher>();
                    x.ConnectImplementationsToTypesClosing(typeof(LiteCQRS.ICommandHandler<>));
                    x.ConnectImplementationsToTypesClosing(typeof(LiteCQRS.IQueryHandler<,>));
                    x.ConnectImplementationsToTypesClosing(typeof(LiteCQRS.IEventHandler<>));
                    x.WithDefaultConventions();
                    x.Exclude(type => type == typeof(ValidationDecorator<>));
                });

                _.For(typeof(ICommandHandler<>)).DecorateAllWith(typeof(ValidationDecorator<>));
            });
        }

        public BootstrappingTests()
        {
            _container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AssemblyContainingType<LiteCQRS.IDispatcher>();
                    x.ConnectImplementationsToTypesClosing(typeof(LiteCQRS.ICommandHandler<>));
                    x.ConnectImplementationsToTypesClosing(typeof(LiteCQRS.IQueryHandler<,>));
                    x.ConnectImplementationsToTypesClosing(typeof(LiteCQRS.IEventHandler<>));
                    x.WithDefaultConventions();
                    x.Exclude(type => type == typeof(ValidationDecorator<>));
                });

                _.For(typeof(ICommandHandler<>)).DecorateAllWith(typeof(ValidationDecorator<>));
            });
        }

        [TestMethod]
        public void InvocationTest()
        {
            var dispatcher = _container.GetInstance<IDispatcher>();

            var result = dispatcher.Execute<GreetingsCommand>(new GreetingsCommand() { Name = "Hola" });
            var queryResult = dispatcher.ExecuteQuery<GetEmployeesQuery, EmployeeResult>(new GetEmployeesQuery() { Name = "Javier" });

            Assert.IsTrue(result.OperationSuccesful == true);
            Assert.IsTrue(queryResult.Employees.Count > 1);
        }
    }
}
