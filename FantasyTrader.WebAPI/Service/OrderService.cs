using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.data;
using FantasyTrader.WebAPI.entities;
using FantasyTrader.WebAPI.HubConfig;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FantasyTrader.WebAPI.Service
{
    public class OrderService
    {
        private readonly FantasyTraderDataContext _context;
        private readonly ILogger<OrderService> _logger;
        private readonly IHubContext<OrderHub> _hub;
        private readonly FantasyMarketPriceSource _prices;
        public OrderService(FantasyTraderDataContext context, ILogger<OrderService> logger, IHubContext<OrderHub> hub, FantasyMarketPriceSource prices)
        {
            _context = context;
            _logger = logger;
            _hub = hub;
            _prices = prices;
        }

        private async Task SendUpdate(Order o)
        {
            await _hub.Clients.All.SendAsync("Order Update", o);
        }

        private async Task CancelOlderOrders()
        {
            _logger.LogInformation("Cleaning up old orders");
            DateTime Today = DateTime.Today;
            var orders = _context.Orders.Where(o =>
                ((o.OrderState == OrderState.New || o.OrderState == OrderState.Partial) && o.Created < Today)).ToList();
            foreach (Order o in orders)
            {
                o.OrderState = o.OrderState == OrderState.New ? OrderState.Cancelled : OrderState.BalanceCancelled;
                await AddSystemHistory(o, "Old Order Cancelled");
                await SendUpdate(o);
            }
            _context.SaveChanges();
        }

        private async Task RejectOrder(Order o, string message)
        {
            o.OrderState = OrderState.Rejected;
            await AddSystemHistory(o, message);
        }

        private async Task AddSystemHistory(Order o, string message)
        {
            //TODO add a system user
            _logger.LogInformation($"Adding Order History for order {o.Id} - {message}");
            await _context.OrderHistories.AddAsync(OrderHistory.CreateFromOrder(o, message, null));
        }

        private decimal CalculateAveragePrice(int oldQuantity, decimal oldPrice, int deltaQuantity, decimal deltaPrice)
        {
            return ((oldQuantity * oldPrice) + (deltaQuantity * deltaPrice)) / (oldQuantity + deltaQuantity);
        }

        private async Task FillOrder(Order o, int fillQuantity, decimal fillPrice)
        {
            o.AverageFillPrice = CalculateAveragePrice(o.FillQuantity, o.AverageFillPrice, fillQuantity, fillPrice);
            await AddSystemHistory(o, $"Filled: {fillQuantity} @ {fillPrice}. New average price is {o.AverageFillPrice}");
            o.FillQuantity += fillQuantity;
            if (o.FillQuantity > o.Quantity)
            {
                o.OrderState = OrderState.Error;
                await AddSystemHistory(o, "Error: order overfilled");
            }
            else if (o.FillQuantity == o.Quantity)
            {
                o.OrderState = OrderState.Filled;
                await AddSystemHistory(o, "Order fully filled");
            }
            else
            {
                o.OrderState = OrderState.Partial;
            }

            _logger.LogInformation($"Looking up the instrument for symbol {o.Symbol}");

            var instrument = await _context.Instruments.FirstOrDefaultAsync(i => i.Symbol == o.Symbol);
            if (instrument == null)
            {
                _logger.LogError($"Unknown symbol: {o.Symbol}");
                return;
            }

            await UpdatePosition(o.Account, instrument, fillQuantity, fillPrice, o.Side);
        }

        private async Task UpdatePosition(Account account, Instrument instrument, int fillQuantity, decimal fillPrice, Side side)
        {
            var pos = await _context.Positions.FirstOrDefaultAsync(pos =>
                (pos.Account == account && pos.Instrument == instrument));
            if (pos == null)
            {
                pos = new Position {Account = account, Instrument = instrument};
                await _context.Positions.AddAsync(pos);
            }
            _logger.LogInformation($"Adding/updating position: {pos}");
            pos.AddFill(fillQuantity, fillPrice, side);
            _logger.LogInformation("Added position");
            await _hub.Clients.All.SendAsync("New Position", pos);
        }

        private async Task FillOrKillOrders()
        {
            _logger.LogInformation("Fill / Killing orders");
            var orders =
                _context.Orders.Where(o => o.OrderState == OrderState.New && o.OrderType == OrderType.FillOrKill).ToList();
            foreach (Order o in orders)
            {
                _logger.LogInformation($"Fill/Killing order: {o}");
                var price = _prices.GetPriceForSymbol(o.Symbol);
                if (price == null)
                {
                    _logger.LogInformation($"Rejecting order: {o.Id}");
                    await RejectOrder(o, "No price found");
                }
                else
                {
                    _logger.LogInformation($"Filling order: {o.Id}");
                    await FillOrder(o, o.Quantity, price.LastPrice);
                }
                _context.SaveChanges();
                await SendUpdate(o);
            }


        }

        public async Task ProcessOrders()
        {
            _logger.LogInformation("Starting processing loop");
            await CancelOlderOrders();
            await FillOrKillOrders();
            
        }

        public async Task CreateOrder(Order order)
        {
            order.Account = await _context.Accounts.FirstOrDefaultAsync(a => a.Name == order.Account.Name);
            await _context.Orders.AddAsync(order);
            _context.SaveChanges();
            await AddSystemHistory(order, "Order Created");
            var response = new OrderControllerResponse
            {
                OrderDetails = order,
                ResponseType = OrderControllerResponseType.Accept
            };
            await _context.OrderControllerResponses.AddAsync(response);
            _context.SaveChanges();
            await _hub.Clients.All.SendAsync("New Order", order);

        }
    }
}
