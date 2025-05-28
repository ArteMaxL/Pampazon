using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DispatchesController : ControllerBase
    {
        private readonly PampazonDbContext _context;

        public DispatchesController(PampazonDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dispatch>>> GetAll()
        {
            var dispatches = await _context.Dispatches
                .Include(d => d.Order)
                .ToListAsync();
            return Ok(dispatches);
        }

        [HttpGet("{dispatchNumber}")]
        public async Task<ActionResult<Dispatch>> Get(string dispatchNumber)
        {
            var dispatch = await _context.Dispatches
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.DispatchNumber == dispatchNumber);

            if (dispatch == null)
                return NotFound();

            return Ok(dispatch);
        }

        [HttpPost]
        public async Task<ActionResult<Dispatch>> Create(Dispatch dispatch)
        {
            // Validate if the order exists and is not already dispatched
            var order = await _context.Orders.FindAsync(dispatch.OrderId);
            if (order == null)
                return BadRequest("Order not found");

            if (order.Status == OrderStatus.Dispatched)
                return BadRequest("Order is already dispatched");

            // Generate dispatch number
            var lastDispatch = await _context.Dispatches
                .OrderByDescending(d => d.DispatchNumber)
                .FirstOrDefaultAsync();

            int counter = 1;
            if (lastDispatch != null && int.TryParse(lastDispatch.DispatchNumber[3..], out int lastNumber))
            {
                counter = lastNumber + 1;
            }

            dispatch.DispatchNumber = $"DSP{counter:D6}";
            dispatch.Date = DateTime.UtcNow;
            dispatch.IsFinalized = false;

            _context.Dispatches.Add(dispatch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { dispatchNumber = dispatch.DispatchNumber }, dispatch);
        }

        [HttpPost("{dispatchNumber}/finalize")]
        public async Task<IActionResult> Finalize(string dispatchNumber)
        {
            var dispatch = await _context.Dispatches
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.DispatchNumber == dispatchNumber);

            if (dispatch == null)
                return NotFound();

            if (dispatch.IsFinalized)
                return BadRequest("Dispatch is already finalized");

            if (dispatch.Order == null)
                return BadRequest("Cannot finalize a dispatch without an order");

            // Update order status
            dispatch.Order.Status = OrderStatus.Dispatched;
            dispatch.IsFinalized = true;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
} 