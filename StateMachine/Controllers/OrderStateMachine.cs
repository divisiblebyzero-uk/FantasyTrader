using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StateMachine.entities;

namespace StateMachine.Controllers
{
    public class OrderStateMachine
    {
        public Order CreateOrder(string clientOrderId, int quantity, string symbol, int accountId)
        {
            Order order = new Order(clientOrderId, quantity,symbol,accountId);
            order.AddOrderHistory(new OrderHistory(this, OrderState.New, OrderState.New, "Order created", null));
            return order;
        }


    }
}
