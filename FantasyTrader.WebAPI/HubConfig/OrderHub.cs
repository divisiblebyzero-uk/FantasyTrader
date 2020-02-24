using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.entities;
using Microsoft.AspNetCore.SignalR;

namespace FantasyTrader.WebAPI.HubConfig
{
    public class OrderHub : Hub
    {
        public async Task SendOrder(Order order)
        {
            await Clients.All.SendAsync("New order", order);
        }
    }
}
