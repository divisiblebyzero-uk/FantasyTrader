using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        [Fact]
        public void OrderCreationAndUpdate()
        {
 
            DateTimeOffset startTime = DateTimeOffset.UtcNow;
            Thread.Sleep(10);
            Order order = new Order("MY ORDER", 100, "ABC", 1);

            TimeSpan difference = order.Created - startTime;
            Assert.True(difference.TotalMilliseconds > 0);
        }
    }
}
