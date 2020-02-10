using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

        public void Initialize()
        {
            
            _context.Database.EnsureCreated();

            var account = GetOrCreateAccount("Default Account");
            var user = GetOrCreateUser("Default User");



            Order order = new Order
            {
                ClientOrderId = "Test Order",
                Quantity = 100,
                Symbol = "ABC",
                Account = account,
                OrderType = OrderType.FillOrKill,
                Side = Side.Buy,
                Price = 100m
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
            

            //_ordersController.CreateOrder(new Order("Test Order 3", 100, "ABC", 1, OrderType.FillOrKill));
        }
    }
}
