using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyTrader.WebAPI.data;
using FantasyTrader.WebAPI.entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FantasyTrader.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PriceGridsController : ControllerBase
    {
        private readonly FantasyTraderDataContext _context;

        public PriceGridsController(FantasyTraderDataContext context)
        {
            _context = context;
        }

        // GET: api/PriceGrids
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceGrid>>> GetPriceGrids()
        {
            return await _context.PriceGrids.ToListAsync();
        }

        // GET: api/PriceGrids/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PriceGrid>> GetPriceGrid(int id)
        {
            var priceGrid = await _context.PriceGrids.FindAsync(id);

            if (priceGrid == null)
            {
                return NotFound();
            }

            return priceGrid;
        }

        // PUT: api/PriceGrids/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPriceGrid(int id, PriceGrid priceGrid)
        {
            if (id != priceGrid.Id)
            {
                return BadRequest();
            }

            _context.Entry(priceGrid).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PriceGridExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PriceGrids
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PriceGrid>> PostPriceGrid(PriceGrid priceGrid)
        {
            _context.PriceGrids.Add(priceGrid);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPriceGrid", new { id = priceGrid.Id }, priceGrid);
        }

        // DELETE: api/PriceGrids/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PriceGrid>> DeletePriceGrid(int id)
        {
            var priceGrid = await _context.PriceGrids.FindAsync(id);
            if (priceGrid == null)
            {
                return NotFound();
            }

            _context.PriceGrids.Remove(priceGrid);
            await _context.SaveChangesAsync();

            return priceGrid;
        }

        private bool PriceGridExists(int id)
        {
            return _context.PriceGrids.Any(e => e.Id == id);
        }
    }
}
