using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachine.entities
{
    public class Order
    {
        public Order()
        {
            FillQuantity = 0;
            OrderState = OrderState.New;
            Created = DateTimeOffset.UtcNow;
            Updated = Created;
        }

        public int Id { get; set; }
        public string ClientOrderId { get; set; }
        public int Quantity { get; set; }
        public string Symbol { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int FillQuantity { get; set; }
        public OrderState OrderState { get; set; }
        public OrderType OrderType { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
       
        public Side Side { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }
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

    public enum Side
    {
        Buy,
        Sell
    }
    
}
