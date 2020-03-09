using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.data;
using FantasyTrader.WebAPI.entities;
using FantasyTrader.WebAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FantasyTrader.WebAPI.HubConfig
{

    [Authorize]
    public class OrderHub : Hub
    {
        private readonly FantasyTraderDataContext _context;
        private readonly OrderService _orderService;
        public OrderHub(FantasyTraderDataContext context, OrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _context.Orders.Include(o => o.Account).ToListAsync();
        }

        public async Task CreateOrder(string orderString)
        {
            Order order = JsonConvert.DeserializeObject<Order>(orderString);
            await _orderService.CreateOrder(order);
            await _orderService.ProcessOrders();
        }
    }

}
