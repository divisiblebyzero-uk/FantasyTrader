using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Logging;
using FantasyTrader.WebAPI.data;
using FantasyTrader.WebAPI.Service;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace Test.FantasyTrader.WebAPI
{
    public class TestFantasyMarketPriceSource
    {
        private readonly ILogger<FantasyMarketPriceSource> _logger;
        private readonly ITestOutputHelper _output;

        public TestFantasyMarketPriceSource(ITestOutputHelper output)
        {
            _output = output;
            _logger = output.BuildLoggerFor<FantasyMarketPriceSource>();
        }

        private FantasyTraderDataContext GetContext(string methodName)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<FantasyTraderDataContext>()
                .UseSqlite(connection)
                .Options;
            var context = new FantasyTraderDataContext(options);
            var dbInitialiser = new DbInitialiser(context, new NullLogger<DbInitialiser>());
            dbInitialiser.Initialize();
            return context;
        }

        [Fact]
        public void TestPriceGeneration()
        {
            /*
            FantasyMarketPriceSource priceSource = new FantasyMarketPriceSource(GetContext("TestPriceGeneration"), _logger);
            decimal lastPrice = 100m;
            for (int i = 0; i < 100; i++)
            {
                decimal newPrice = priceSource.GetNextPrice("TEST");
                Assert.True(Math.Abs((newPrice-lastPrice) / lastPrice) < 0.6m);
                lastPrice = newPrice;
            }
            */
            
        }
    }
}
