using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachine.entities
{
    public class Instrument
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public InstrumentPriceSource InstrumentPriceSource { get; set; }
    }

    public enum InstrumentPriceSource
    {
        FantasyMarket,
        AlphaVantage
    }
}
