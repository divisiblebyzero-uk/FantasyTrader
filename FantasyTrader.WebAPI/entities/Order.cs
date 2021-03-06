﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyTrader.WebAPI.entities
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
        public decimal LimitPrice { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AverageFillPrice { get; set; }
        [NotMapped] public decimal PercentComplete => Decimal.Divide(FillQuantity, Quantity);

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ClientOrderId)}: {ClientOrderId}, {nameof(Quantity)}: {Quantity}, {nameof(Symbol)}: {Symbol}, {nameof(AccountId)}: {AccountId}, {nameof(Account)}: {Account}, {nameof(FillQuantity)}: {FillQuantity}, {nameof(OrderState)}: {OrderState}, {nameof(OrderType)}: {OrderType}, {nameof(Created)}: {Created}, {nameof(Updated)}: {Updated}, {nameof(Side)}: {Side}, {nameof(LimitPrice)}: {LimitPrice}, {nameof(AverageFillPrice)}: {AverageFillPrice}, {nameof(PercentComplete)}: {PercentComplete}";
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
        Error,
        BalanceCancelled,
        Rejected
    }

    public enum Side
    {
        Buy,
        Sell
    }
    
}
