using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyTrader.WebAPI.entities
{
    public class Price
    {
        public string Symbol { get; set; }
        public decimal LastPrice { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int Direction { get; set; }
    }
}
