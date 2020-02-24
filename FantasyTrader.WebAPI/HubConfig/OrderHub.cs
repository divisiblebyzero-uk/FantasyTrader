using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.data;
using FantasyTrader.WebAPI.entities;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace FantasyTrader.WebAPI.HubConfig
{

    public class OrderHub : Hub
    {
        private FantasyTraderDataContext _context;
        public OrderHub(FantasyTraderDataContext context)
        {
            _context = context;
        }

        public async Task SendOrder(Order order)
        {
            await Clients.All.SendAsync("New order", order);
        }

        public string SayHello(string name)
        {
            Order o = JsonConvert.DeserializeObject<Order>(name);
            return "Hello: " + name;
        }
        public string CreateOrder(string orderString)
        {
            Order order = JsonConvert.DeserializeObject<Order>(orderString);
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
            Clients.All.SendAsync("New Order", order);
            //return response;
            return "OK";
        }
    }

}
