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

        private void SendUpdate(Order o)
        {
            _hub.Clients.All.SendAsync("Order Update", o);
        }

        private void CancelOlderOrders()
        {
            _logger.LogInformation("Cleaning up old orders");
            DateTime Today = DateTime.Today;
            var orders = _context.Orders.Where(o =>
                ((o.OrderState == OrderState.New || o.OrderState == OrderState.Partial) && o.Created < Today)).ToList();
            foreach (Order o in orders)
            {
                o.OrderState = o.OrderState == OrderState.New ? OrderState.Cancelled : OrderState.BalanceCancelled;
                _context.OrderHistories.Add(OrderHistory.CreateFromOrder(o, "Old order cancelled", null));
                SendUpdate(o);
            }
            _context.SaveChanges();
        }

        private void RejectOrder(Order o, string message)
        {
            o.OrderState = OrderState.Rejected;
            _context.OrderHistories.Add(OrderHistory.CreateFromOrder(o, message, null));
        }

        private void FillOrKillOrders()
        {
            _logger.LogInformation("Fill / Killing orders");
            var orders =
                _context.Orders.Where(o => o.OrderState == OrderState.New && o.OrderType == OrderType.FillOrKill);
            foreach (Order o in orders)
            {
                var price = _prices.GetPriceForSymbol(o.Symbol);
                if (price == null)
                {
                    RejectOrder(o, "No price found");
                }
                else
                {
                    o.AverageFillPrice = price.LastPrice;
                    o.FillQuantity = o.Quantity;
                    o.OrderState = OrderState.Filled;
                    
                    _context.OrderHistories.Add(OrderHistory.CreateFromOrder(o, "Filled", null));
                }

                SendUpdate(o);
            }

            _context.SaveChanges();
        }

        public Task ProcessOrders()
        {
            _logger.LogInformation("Starting processing loop");
            CancelOlderOrders();
            FillOrKillOrders();
            return Task.CompletedTask;
        }

        public async Task CreateOrder(Order order)
        {
            order.Account = _context.Accounts.FirstOrDefault(a => a.Name == order.Account.Name);
            _context.Orders.Add(order);
            _context.OrderHistories.Add(OrderHistory.CreateFromOrder(order, "Order created", null));
            var response = new OrderControllerResponse
            {
                OrderDetails = order,
                ResponseType = OrderControllerResponseType.Accept
            };
            _context.OrderControllerResponses.Add(response);
            _context.SaveChanges();
            await _hub.Clients.All.SendAsync("New Order", order);

        }
    }
}
