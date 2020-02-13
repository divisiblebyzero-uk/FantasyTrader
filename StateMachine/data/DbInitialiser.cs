using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using StateMachine.Controllers;
using StateMachine.entities;

namespace StateMachine.data
{
    public class DbInitialiser
    {
        private readonly StateMachineDataContext _context;
        private readonly ILogger<DbInitialiser> _logger;
        private readonly OrdersController _ordersController;

        public DbInitialiser(StateMachineDataContext context, ILogger<DbInitialiser> logger)//, OrdersController ordersController)
        {
            _context = context;
            _logger = logger;
            //_ordersController = ordersController;
        }

        private Account GetOrCreateAccount(string name)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Name == name);
            if (account == null)
            {
                account = new Account(name);
                _context.Accounts.Add(account);
                _context.SaveChanges();
            }

            return account;
        }

        private User GetOrCreateUser(string name)
        {
            var user = _context.Users.FirstOrDefault(a => a.Name == name);
            if (user == null)
            {
                user = new User(name);
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            return user;
        }

        private void CreateInstrumentsIfNecessary()
        {
            if (_context.Instruments.Count() < 10)
            {
                string[] symbols = new string[] { "MSFT", "AAPL", "GOOG", "TSLA", "CSCO", "PEP", "C", "DAVA", "SQ", "KHC" };
                foreach (string symbol in symbols)
                {
                    GetOrCreateInstrument(symbol, InstrumentPriceSource.AlphaVantage);
                }
            }
        }

        private Instrument GetOrCreateInstrument(String symbol, InstrumentPriceSource instrumentPriceSource)
        {
            var instrument = _context.Instruments.FirstOrDefault(i => i.Symbol == symbol);
            if (instrument == null)
            {
                instrument = new Instrument
                {
                    Symbol = symbol,
                    InstrumentPriceSource = instrumentPriceSource
                };
                _context.Instruments.Add(instrument);
            }

            _context.SaveChanges();
            return instrument;
        }

        public void Initialize()
        {

            _context.Database.EnsureCreated();

            var account = GetOrCreateAccount("Default Account");
            var user = GetOrCreateUser("Default User");
            CreateInstrumentsIfNecessary();
            var instrument = GetOrCreateInstrument("FNTSY", InstrumentPriceSource.FantasyMarket);
            CreateOrdersIfNecessary(account, instrument);
        }

        private void CreateOrdersIfNecessary(Account account, Instrument instrument) { 

            if (_context.Orders.Count() < 5)
            {
                Order order = new Order
                {
                    ClientOrderId = "Test Order",
                    Quantity = 100,
                    Symbol = instrument.Symbol,
                    Account = account,
                    OrderType = OrderType.FillOrKill,
                    Side = Side.Buy,
                    LimitPrice = 100m
                };
                _context.Orders.Add(order);
                _context.OrderHistories.Add(OrderHistory.CreateFromOrder(order, "Order created", null));
                _context.SaveChanges();

                var response = new OrderControllerResponse
                {
                    OrderDetails = order,
                    ResponseType = OrderControllerResponseType.Accept
                };
                _context.OrderControllerResponses.Add(response);
                _context.SaveChanges();
            }

        }
    }
}
