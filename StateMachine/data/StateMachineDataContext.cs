using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StateMachine.entities;

namespace StateMachine.data
{
    public class StateMachineDataContext : DbContext
    {
        public StateMachineDataContext(DbContextOptions<StateMachineDataContext> options) : base(options)
        {

        }

        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<OrderControllerResponse> OrderControllerResponses { get; set; }
    }
}
