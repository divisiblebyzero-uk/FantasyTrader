using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.data;
using FantasyTrader.WebAPI.entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FantasyTrader.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController
    {
        private readonly ILogger<PositionsController> _logger;
        private readonly FantasyTraderDataContext _context;

        public PositionsController(ILogger<PositionsController> logger, FantasyTraderDataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("accountId={accountId}")]
        public async Task<ActionResult<IEnumerable<Position>>> GetPositionsForAccount(int accountId)
        {
            return await _context.Positions.Where(pos => pos.Account.Id == accountId).ToListAsync();
        }
    }
}
