using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.entities;
using FantasyTrader.WebAPI.Service;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FantasyTrader.WebAPI.HubConfig
{
    public class PriceHub : Hub
    {

        private readonly FantasyMarketPriceSource _fantasyMarketPriceSource;
        private ILogger<PriceHub> _logger;

        public PriceHub(FantasyMarketPriceSource fantasyMarketPriceSource, ILogger<PriceHub> logger)
        {
            _fantasyMarketPriceSource = fantasyMarketPriceSource;
            _logger = logger;
        }

        public IEnumerable<Price> GetAllPrices()
        {
            return _fantasyMarketPriceSource.GetAllPrices();
        }

        public ChannelReader<Price> StreamPrices()
        {
            return _fantasyMarketPriceSource.StreamPrices().AsChannelReader(10);
        }

        public string GetMarketState()
        {
            return _fantasyMarketPriceSource.MarketState.ToString();
        }

        public async Task OpenMarket()
        {
            _logger.LogInformation("Opening market");
            await _fantasyMarketPriceSource.OpenMarket();
        }

        public async Task CloseMarket()
        {
            _logger.LogInformation("Closing market");
            await _fantasyMarketPriceSource.CloseMarket();
        }

        public async Task Reset()
        {
            await _fantasyMarketPriceSource.Reset();
        }
    }
}
