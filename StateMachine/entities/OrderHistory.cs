using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachine.entities
{
    public class OrderHistory: OrderDetails
    {
        private OrderHistory()
        {

        }

        public static OrderHistory CreateFromOrderDetails(OrderDetails orderDetails, string message, User user)
        {
            OrderHistory history = new OrderHistory();
            CopyFields<OrderDetails>(orderDetails, history);
            history.Message = message;
            history.User = user;
            history.Timestamp = DateTimeOffset.UtcNow;
            return history;
        }

        public int OrderHistoryId { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public static void CopyFields<T>(T source, T destination)
        {
            var fields = source.GetType().GetFields();
            foreach (var field in fields)
            {
                field.SetValue(destination, field.GetValue(source));
            }
        }

    }
}
