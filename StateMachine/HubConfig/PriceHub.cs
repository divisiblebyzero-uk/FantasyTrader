using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.entities;
using FantasyTrader.WebAPI.Service;
using Microsoft.AspNetCore.SignalR;

namespace FantasyTrader.WebAPI.HubConfig
{
    public class PriceHub : Hub
    {
        private class PriceObserver : IObserver<Price>
        {
            private IClientProxy _client;
            private string _symbol;

            public PriceObserver(IClientProxy client, string symbol)
            {
                _client = client;
                _symbol = symbol;
            }

            public void OnCompleted()
            {
                throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnNext(Price value)
            {
                _client.SendAsync("Price update", value);
            }
        }
        private readonly PriceDistributionService _priceDistributionService;
        public PriceHub(PriceDistributionService priceDistributionService)
        {
            _priceDistributionService = priceDistributionService;
        }
        public async Task Subscribe(string symbol)
        {
            await Clients.Caller.SendAsync("Subscription accepted", symbol);
            _priceDistributionService.GetTracker(symbol).Subscribe(new PriceObserver(Clients.Caller, symbol));
            
        }
    }
}
