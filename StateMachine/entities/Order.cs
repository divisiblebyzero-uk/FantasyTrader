using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachine.entities
{
    public class Order
    {
        public Order(string clientOrderId, int quantity, string symbol, int accountId)
        {
            ClientOrderId = clientOrderId;
            Quantity = quantity;
            Symbol = symbol;
            AccountId = accountId;
            FillQuantity = 0;
            _orderHistory = new HashSet<OrderHistory>();
            
            OrderState = OrderState.New;
        }

        public int Id { get; set; }
        public string ClientOrderId { get; set; }
        public int Quantity { get; set; }
        public string Symbol { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int FillQuantity { get; set; }
        public OrderState OrderState { get; set; }

        public DateTimeOffset Created => OrderHistory.OrderByDescending(h => h.Created).First().Created;
        public DateTimeOffset Updated => OrderHistory.OrderBy(h => h.Created).First().Created;

        public IEnumerable<OrderHistory> OrderHistory => _orderHistory.ToList();

        public void AddOrderHistory(OrderHistory orderHistory)
        {
            _orderHistory.Add(orderHistory);
        }

        private HashSet<OrderHistory> _orderHistory { get; }
        
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
