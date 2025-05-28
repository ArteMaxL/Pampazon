using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly PampazonDbContext _context;

        public StockController(PampazonDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockPosition>>> GetAll()
        {
            var positions = await _context.StockPositions
                .Include(p => p.Product)
                .Include(p => p.Client)
                .ToListAsync();
            return Ok(positions);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<StockPosition>>> GetByProduct(string productId)
        {
            var positions = await _context.StockPositions
                .Include(p => p.Product)
                .Include(p => p.Client)
                .Where(p => p.ProductId == productId)
                .ToListAsync();
            return Ok(positions);
        }

        [HttpPost]
        public async Task<ActionResult<StockPosition>> Create(StockPosition position)
        {
            // Validate position doesn't exist
            var exists = await _context.StockPositions.AnyAsync(p => 
                p.Aisle == position.Aisle && 
                p.Section == position.Section && 
                p.Shelf == position.Shelf && 
                p.Level == position.Level);

            if (exists)
            {
                return Conflict("This position is already in use");
            }

            _context.StockPositions.Add(position);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByProduct), new { productId = position.ProductId }, position);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StockPosition position)
        {
            if (id != position.Id)
                return BadRequest();

            var existingPosition = await _context.StockPositions.FindAsync(id);
            if (existingPosition == null)
                return NotFound();

            _context.Entry(existingPosition).CurrentValues.SetValues(position);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await StockPositionExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var position = await _context.StockPositions.FindAsync(id);
            if (position == null)
                return NotFound();

            _context.StockPositions.Remove(position);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> StockPositionExists(int id)
        {
            return await _context.StockPositions.AnyAsync(p => p.Id == id);
        }
    }
} 