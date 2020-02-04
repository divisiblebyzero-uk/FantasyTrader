using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachine.entities
{
    public class OrderHistory
    {
        public OrderHistory(Order order, OrderState fromState, OrderState state, string message, User user)
        {
            Order = order;
            FromState = fromState;
            ToState = state;
            Message = message;
            User = user;
            Created = DateTimeOffset.UtcNow;
        }

        public int Id { get; set; }
        public Order Order { get; set; }
        public OrderState FromState { get; set; }
        public OrderState ToState { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
