using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StateMachine.data;
using StateMachine.entities;

namespace StateMachine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceGridEntriesController : ControllerBase
    {
        private readonly StateMachineDataContext _context;

        public PriceGridEntriesController(StateMachineDataContext context)
        {
            _context = context;
        }

        // GET: api/PriceGridEntries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceGridEntry>>> GetPriceGridEntries()
        {
            return await _context.PriceGridEntries.ToListAsync();
        }

        // GET: api/PriceGridEntries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PriceGridEntry>> GetPriceGridEntry(int id)
        {
            var priceGridEntry = await _context.PriceGridEntries.FindAsync(id);

            if (priceGridEntry == null)
            {
                return NotFound();
            }

            return priceGridEntry;
        }

        // PUT: api/PriceGridEntries/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPriceGridEntry(int id, PriceGridEntry priceGridEntry)
        {
            if (id != priceGridEntry.Id)
            {
                return BadRequest();
            }

            _context.Entry(priceGridEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PriceGridEntryExists(id))
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

        // POST: api/PriceGridEntries
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PriceGridEntry>> PostPriceGridEntry(PriceGridEntry priceGridEntry)
        {
            _context.PriceGridEntries.Add(priceGridEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPriceGridEntry", new { id = priceGridEntry.Id }, priceGridEntry);
        }

        // DELETE: api/PriceGridEntries/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PriceGridEntry>> DeletePriceGridEntry(int id)
        {
            var priceGridEntry = await _context.PriceGridEntries.FindAsync(id);
            if (priceGridEntry == null)
            {
                return NotFound();
            }

            _context.PriceGridEntries.Remove(priceGridEntry);
            await _context.SaveChangesAsync();

            return priceGridEntry;
        }

        private bool PriceGridEntryExists(int id)
        {
            return _context.PriceGridEntries.Any(e => e.Id == id);
        }
    }
}
