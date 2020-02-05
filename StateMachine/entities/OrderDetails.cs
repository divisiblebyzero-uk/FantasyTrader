using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachine.entities
{
    public class OrderDetails
    {
        protected OrderDetails()
        {

        }

        public OrderDetails(string clientOrderId, int quantity, string symbol, int accountId, OrderType orderType)
        {
            ClientOrderId = clientOrderId;
            Quantity = quantity;
            Symbol = symbol;
            AccountId = accountId;
            FillQuantity = 0;
            OrderHistory = new HashSet<OrderHistory>();
            OrderType = orderType;
            OrderState = OrderState.New;
        }

        public int OrderDetailsId { get; set; }
        public string ClientOrderId { get; set; }
        public int Quantity { get; set; }
        public string Symbol { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int FillQuantity { get; set; }
        public OrderState OrderState { get; set; }
        public OrderType OrderType { get; set; }

        public DateTimeOffset GetCreated()
        {
            return OrderHistory.OrderByDescending(h => h.Timestamp).First().Timestamp;
        }

        public DateTimeOffset GetUpdated()
        {
            return OrderHistory.OrderBy(h => h.Timestamp).First().Timestamp;
        }

        public HashSet<OrderHistory> OrderHistory { get; set; }

        public void AddOrderHistory(OrderHistory orderHistory)
        {
            OrderHistory.Add(orderHistory);
        }
        
    }

    public enum OrderType
    {
        FillOrKill
    }

    public enum OrderState
    {
        New,
        Partial,
        Filled,
        Cancelled,
        Error
    }
    
}
