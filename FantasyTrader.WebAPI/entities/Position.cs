using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyTrader.WebAPI.entities
{
    public class Position
    {
        public int Id { get; set; }
        public Account Account { get; set; }
        public Instrument Instrument { get; set; }
        public int LongQuantity { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal LongValue { get; set; }
        public int ShortQuantity { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal ShortValue { get; set; }
        [NotMapped] public decimal? LongAveragePrice => GetAveragePrice(LongQuantity, LongValue);
        [NotMapped] public decimal? ShortAveragePrice => GetAveragePrice(ShortQuantity, ShortValue);
        [NotMapped] public int NetQuantity => LongQuantity - ShortQuantity;

        public void AddFill(int fillQuantity, decimal fillPrice, Side side)
        {
            if (side == Side.Buy)
            {
                LongQuantity += fillQuantity;
                LongValue += (fillQuantity * fillPrice);
            }
            else
            {
                ShortQuantity += fillQuantity;
                ShortValue += (fillQuantity * fillPrice);
            }
        }

        private static decimal? GetAveragePrice(int quantity, decimal value)
        {
            if (quantity == 0)
            {
                return null;
            }
            else
            {
                return value / quantity;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Account)}: {Account}, {nameof(Instrument)}: {Instrument}, {nameof(LongQuantity)}: {LongQuantity}, {nameof(LongValue)}: {LongValue}, {nameof(ShortQuantity)}: {ShortQuantity}, {nameof(ShortValue)}: {ShortValue}, {nameof(LongAveragePrice)}: {LongAveragePrice}, {nameof(ShortAveragePrice)}: {ShortAveragePrice}, {nameof(NetQuantity)}: {NetQuantity}";
        }
    }
}
