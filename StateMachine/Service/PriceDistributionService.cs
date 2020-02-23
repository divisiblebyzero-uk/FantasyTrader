using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.entities;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace FantasyTrader.WebAPI.Service
{
    public class PriceTracker : IObservable<Price>
    {
        private readonly List<IObserver<Price>> _observers;

        public PriceTracker()
        {
            _observers = new List<IObserver<Price>>();
        }

        public IDisposable Subscribe(IObserver<Price> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<Price>> _observers;
            private IObserver<Price> _observer;

            public Unsubscriber(List<IObserver<Price>> observers, IObserver<Price> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }

        public void SendNewPrice(Price price)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(price);
            }
        }

        public void EndTransmission()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }

            _observers.Clear();
        }

        public Boolean HasSubscribers()
        {
            return _observers.Count > 0;
        }
    }

    public class PriceDistributionService : IHostedService, IDisposable
    {
        private readonly Dictionary<string, PriceTracker> _trackers = new Dictionary<string, PriceTracker>();
        private readonly FantasyMarketPriceSource _fantasyMarketPriceSource;
        private Timer _timer;

        public PriceDistributionService(FantasyMarketPriceSource fantasyMarketPriceSource)
        {
            _fantasyMarketPriceSource = fantasyMarketPriceSource;
        }

        public PriceTracker GetTracker(string symbol)
        {
            if (!_trackers.ContainsKey(symbol))
            {
                _trackers[symbol] = new PriceTracker();
            }

            return _trackers[symbol];
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(GetNewPrices, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void GetNewPrices(object state)
        {
            foreach (string symbol in _trackers.Keys)
            {
                if (_trackers[symbol].HasSubscribers())
                {
                    _trackers[symbol].SendNewPrice(new Price()
                    {
                        LastPrice = _fantasyMarketPriceSource.GetNextPrice(symbol),
                        Symbol = symbol,
                        Timestamp = DateTimeOffset.Now
                    });
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
