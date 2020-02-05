using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StateMachine.data;
using StateMachine.entities;

namespace StateMachine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController
    {
        private readonly StateMachineDataContext _context;

        public OrderController(StateMachineDataContext context)
        {
            _context = context;
        }

        public OrderControllerResponse CreateOrder(string clientOrderId, int quantity, string symbol, int accountId, OrderType orderType)
        {
            OrderDetails order = new OrderDetails(clientOrderId, quantity, symbol, accountId, orderType);
            order.AddOrderHistory(OrderHistory.CreateFromOrderDetails(order, "Order created", null));
            _context.OrderDetails.Add(order);
            var response = new OrderControllerResponse
            {
                OrderDetails = order, ResponseType = OrderControllerResponseType.Accept
            };
            _context.OrderControllerResponses.Add(response);
            _context.SaveChanges();
            return response;
        }

        /*
        public OrderDetails AmendOrder(string clientOrderId, int quantity, string symbol, int accountId,
            OrderType orderType)
        {
            OrderDetails orderDetails = _context.OrderDetails.FirstOrDefault(o => o.ClientOrderId == clientOrderId);
            if (orderDetails == null)
            {
                throw new Exception("error - no order found");
            }
        }
        */


    }
}
