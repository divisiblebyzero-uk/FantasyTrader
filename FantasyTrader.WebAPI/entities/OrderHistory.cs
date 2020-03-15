using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FantasyTrader.WebAPI.entities
{
    public class OrderHistory
    {
        private OrderHistory()
        {

        }

        public static OrderHistory CreateFromOrder(Order order, string message, User user)
        {
            OrderHistory history = new OrderHistory
            {
                OrderId = order.Id,
                OrderDetailsSnapshot = JsonConvert.SerializeObject(order),
                Message = message,
                User = user,
                Timestamp = DateTimeOffset.UtcNow
            };
            return history;
        }

        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderDetailsSnapshot { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
        public DateTimeOffset Timestamp { get; set; }

    }
}
