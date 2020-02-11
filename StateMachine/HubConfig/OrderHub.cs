using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using StateMachine.entities;

namespace StateMachine.HubConfig
{
    public class OrderHub : Hub
    {
        public async Task SendOrder(Order order)
        {
            await Clients.All.SendAsync("New order", order);
        }
    }
}
