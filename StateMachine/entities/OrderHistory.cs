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
            OrderHistory history = new OrderHistory();
            history.OrderDetailsSnapshot = JsonConvert.SerializeObject(order);
            history.Message = message;
            history.User = user;
            history.Timestamp = DateTimeOffset.UtcNow;
            return history;
        }

        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderDetailsSnapshot { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public static void CopyFields<T, U>(T source, U destination)
        {
            var fields = source.GetType().GetFields();
            foreach (var field in fields)
            {
                field.SetValue(destination, field.GetValue(source));
            }
        }

    }
}
