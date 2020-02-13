using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StateMachine.Controllers;
using StateMachine.data;
using StateMachine.entities;
using StateMachine.HubConfig;
using Xunit;
using Xunit.Abstractions;

namespace Test.StateMachine
{
    public class TestEntities
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestEntities(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        private StateMachineDataContext GetContext(string methodName)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<StateMachineDataContext>()
                .UseSqlite(connection)
                .Options;
            var context = new StateMachineDataContext(options);
            var dbInitialiser = new DbInitialiser(context, new NullLogger<DbInitialiser>());
            dbInitialiser.Initialize();
            return context;
        }

        [Fact]
        public void OrderCreationAndUpdate()
        {
            Mock<IHubClients> mockClients = new Mock<IHubClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            Mock<IHubContext<OrderHub>> mockHubContext = new Mock<IHubContext<OrderHub>>();
            mockHubContext.Setup(c => c.Clients).Returns(() => mockClients.Object);

            DateTimeOffset startTime = DateTimeOffset.UtcNow;
            Thread.Sleep(10);
            using var context = GetContext("blah");
            OrdersController oc = new OrdersController(context, mockHubContext.Object);
            var account = context.Accounts.FirstOrDefault();
            Order order = new Order
            {
                ClientOrderId = "Test Order2",
                Quantity = 100,
                Symbol = "ABC",
                Account = account,
                OrderType = OrderType.FillOrKill,
                Side = Side.Buy,
                LimitPrice = 100m
            };
            
            oc.CreateOrder(order);

            TimeSpan difference = order.Created - startTime;
            Assert.True(difference.TotalMilliseconds > 0);
            mockClients.Verify(clients => clients.All, Times.Once);
        }
    }
}
