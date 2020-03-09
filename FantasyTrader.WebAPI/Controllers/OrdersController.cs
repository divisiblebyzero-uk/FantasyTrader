using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.data;
using FantasyTrader.WebAPI.entities;
using FantasyTrader.WebAPI.HubConfig;
using FantasyTrader.WebAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FantasyTrader.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly FantasyTraderDataContext _context;
        private readonly IHubContext<OrderHub> _hub;
        private readonly OrderService _orderService;

        public OrdersController(FantasyTraderDataContext context, IHubContext<OrderHub> hub, OrderService orderService)
        {
            _context = context;
            _hub = hub;
            _orderService = orderService;
        }

        [HttpGet("process")]
        public async Task<ActionResult> ProcessOrders()
        {
            await _orderService.ProcessOrders();
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        [HttpGet("orderId={orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            else return order;
        }

        /*[HttpGet("clientOrderId={clientOrderId}")]
        public async Task<ActionResult<Order>> GetOrderByClientOrderId(String clientOrderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.ClientOrderId == clientOrderId);
            if (order == null)
            {
                return NotFound();
            }
            else return order;
        }*/

        [HttpPost]
        public async Task<ActionResult<OrderControllerResponse>> CreateOrder(Order order)
        {
            order.Account = _context.Accounts.FirstOrDefault(a => a.Name == order.Account.Name);
            _context.Orders.Add(order);
            _context.OrderHistories.Add(OrderHistory.CreateFromOrder(order, "Order created", null));
            var response = new OrderControllerResponse
            {
                OrderDetails = order, ResponseType = OrderControllerResponseType.Accept
            };
            _context.OrderControllerResponses.Add(response);
            _context.SaveChanges();
            await _hub.Clients.All.SendAsync("New Order", order);
            return response;
        }

        /*
        public Orders AmendOrder(string clientOrderId, int quantity, string symbol, int accountId,
            OrderType orderType)
        {
            Orders orderDetails = _context.Orders.FirstOrDefault(o => o.ClientOrderId == clientOrderId);
            if (orderDetails == null)
            {
                throw new Exception("error - no order found");
            }
        }
        */


    }
}
