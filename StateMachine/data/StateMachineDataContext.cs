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
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<OrderControllerResponse> OrderControllerResponses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<PriceGrid> PriceGrids { get; set; }
        public DbSet<PriceGridEntry> PriceGridEntries { get; set; }
    }
}
