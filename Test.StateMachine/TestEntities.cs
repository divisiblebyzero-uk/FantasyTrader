using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StateMachine.Controllers;
using StateMachine.data;
using StateMachine.entities;
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
            var options = new DbContextOptionsBuilder<StateMachineDataContext>()
                .UseInMemoryDatabase(databaseName: methodName)
                .Options;
            return new StateMachineDataContext(options);
        }


        [Fact]
        public void OrderCreationAndUpdate()
        {
 
            DateTimeOffset startTime = DateTimeOffset.UtcNow;
            Thread.Sleep(10);
            using var context = GetContext("blah");
            OrderController oc = new OrderController(context);

            OrderDetails order = oc.CreateOrder("MY ORDER", 100, "ABC", 1, OrderType.FillOrKill).OrderDetails;

            TimeSpan difference = order.GetCreated() - startTime;
            Assert.True(difference.TotalMilliseconds > 0);
        }
    }
}
