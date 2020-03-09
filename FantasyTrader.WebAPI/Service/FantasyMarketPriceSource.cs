using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.data;
using Microsoft.AspNetCore.SignalR;
using FantasyTrader.WebAPI.HubConfig;
using FantasyTrader.WebAPI.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FantasyTrader.WebAPI.Service
{
    public static class ObservableExtensions
    {
        public static ChannelReader<T> AsChannelReader<T>(this IObservable<T> observable, int? maxBufferSize = null)
        {
            // This sample shows adapting an observable to a ChannelReader without 
            // back pressure, if the connection is slower than the producer, memory will
            // start to increase.

            // If the channel is bounded, TryWrite will return false and effectively
            // drop items.

            // The other alternative is to use a bounded channel, and when the limit is reached
            // block on WaitToWriteAsync. This will block a thread pool thread and isn't recommended and isn't shown here.
            var channel = maxBufferSize != null ? Channel.CreateBounded<T>(maxBufferSize.Value) : Channel.CreateUnbounded<T>();

            var disposable = observable.Subscribe(
                value => channel.Writer.TryWrite(value),
                error => channel.Writer.TryComplete(error),
                () => channel.Writer.TryComplete());

            // Complete the subscription on the reader completing
            channel.Reader.Completion.ContinueWith(task => disposable.Dispose());

            return channel.Reader;
        }
    }
    public class FantasyMarketPriceSource
    {
        private readonly SemaphoreSlim _marketStateLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _updatePricesLock = new SemaphoreSlim(1, 1);

        private readonly ConcurrentDictionary<string, Price> _prices = new ConcurrentDictionary<string, Price>();

        private readonly Subject<Price> _subject = new Subject<Price>();

        private readonly ILogger<FantasyMarketPriceSource> _logger;

        // Stock can go up or down by a percentage of this factor on each change
        private readonly double _rangePercent = 0.002;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random _updateOrNotRandom = new Random();

        private Timer _timer;
        private volatile bool _updatingPrices;
        private volatile MarketState _marketState;

        public FantasyMarketPriceSource(IHubContext<PriceHub> hub, ILogger<FantasyMarketPriceSource> logger)
        {
            Hub = hub;
            _logger = logger;
            LoadDefaultSymbols();
        }

        private IHubContext<PriceHub> Hub
        {
            get;
            set;
        }

        public MarketState MarketState
        {
            get { return _marketState; }
            private set { _marketState = value; }
        }

        public Price GetPriceForSymbol(String symbol)
        {
            return _prices[symbol];
        }

        public IEnumerable<Price> GetAllPrices()
        {
            return _prices.Values;
        }

        public IObservable<Price> StreamPrices()
        {
            return _subject;
        }

        public async Task OpenMarket()
        {
            await _marketStateLock.WaitAsync();
            try
            {
                if (MarketState != MarketState.Open)
                {
                    _timer = new Timer(UpdatePrices, null, _updateInterval, _updateInterval);

                    MarketState = MarketState.Open;

                    await BroadcastMarketStateChange(MarketState.Open);
                }
            }
            finally
            {
                _marketStateLock.Release();
            }
        }

        public async Task CloseMarket()
        {
            await _marketStateLock.WaitAsync();
            try
            {
                if (MarketState == MarketState.Open)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                    }

                    MarketState = MarketState.Closed;

                    await BroadcastMarketStateChange(MarketState.Closed);
                }
            }
            finally
            {
                _marketStateLock.Release();
            }
        }

        public async Task Reset()
        {
            await _marketStateLock.WaitAsync();
            try
            {
                if (MarketState != MarketState.Closed)
                {
                    throw new InvalidOperationException("Market must be closed before it can be reset.");
                }

                LoadDefaultSymbols();
                await BroadcastMarketReset();
            }
            finally
            {
                _marketStateLock.Release();
            }
        }

        private void LoadDefaultSymbols()
        {
            _prices.Clear();

            var now = DateTimeOffset.Now;


            var prices = new List<Price>();
            foreach (string symbol in new string[] { "MSFT", "AAPL", "GOOG", "TSLA", "CSCO", "PEP", "C", "DAVA", "SQ", "KHC", "FNTSY" })
            {
                var p = new Price
                {
                    Symbol = symbol,
                    LastPrice = 100m,
                    Timestamp = now
                };
                prices.Add(p);
            }

            prices.ForEach(price => _prices.TryAdd(price.Symbol, price));
        }

        private async void UpdatePrices(object state)
        {
            _logger.LogDebug("Updating prices");
            // This function must be re-entrant as it's running as a timer interval handler
            await _updatePricesLock.WaitAsync();
            try
            {
                if (!_updatingPrices)
                {
                    _updatingPrices = true;

                    foreach (var price in _prices.Values)
                    {
                        TryUpdatePrice(price);

                        _subject.OnNext(price);
                    }

                    _updatingPrices = false;
                }
            }
            finally
            {
                _updatePricesLock.Release();
            }
        }

        private bool TryUpdatePrice(Price price)
        {
            // Randomly choose whether to udpate this stock or not
            var r = _updateOrNotRandom.NextDouble();
            if (r > 0.1)
            {
                return false;
            }

            // Update the stock price by a random factor of the range percent
            var random = new Random((int)Math.Floor(price.LastPrice));
            var percentChange = random.NextDouble() * _rangePercent;
            var pos = random.NextDouble() > 0.51;
            var change = Math.Round(price.LastPrice * (decimal)percentChange, 2);
            change = pos ? change : -change;

            price.LastPrice += change;
            price.Direction = pos ? 1 : -1;
            price.Timestamp = DateTimeOffset.Now;
            return true;
        }

        private async Task BroadcastMarketStateChange(MarketState marketState)
        {
            switch (marketState)
            {
                case MarketState.Open:
                    await Hub.Clients.All.SendAsync("marketOpened");
                    break;
                case MarketState.Closed:
                    await Hub.Clients.All.SendAsync("marketClosed");
                    break;
                default:
                    break;
            }
        }

        private async Task BroadcastMarketReset()
        {
            await Hub.Clients.All.SendAsync("marketReset");
        }
    }

    public enum MarketState
    {
        Closed,
        Open
    }
}
