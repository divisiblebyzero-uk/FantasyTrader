using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FantasyTrader.WebAPI.data;
using Microsoft.Extensions.Logging;

namespace FantasyTrader.WebAPI.Service
{
    public class FantasyMarketPriceSource
    {
        private readonly string[] _symbols = { "FNTSY" };
        private readonly ILogger<FantasyMarketPriceSource> _logger;
        private readonly Random _random = new Random();
        private readonly Dictionary<string, decimal> _currentPrices;

        public FantasyMarketPriceSource(ILogger<FantasyMarketPriceSource> logger)
        {
            _logger = logger;
            _currentPrices = new Dictionary<string, decimal>();
        }

        public decimal GetNextPrice(string symbol)
        {
            decimal lastPrice = _currentPrices.ContainsKey(symbol) ? _currentPrices[symbol] : 100m;
            double incrementFactor = (_random.NextDouble() - 0.5)/10; // + or - 10%
            _currentPrices[symbol] = lastPrice * (decimal) (1 + incrementFactor);
            return _currentPrices[symbol];
        }

    }
}
