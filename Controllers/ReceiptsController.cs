using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptsController : ControllerBase
    {
        private readonly PampazonDbContext _context;

        public ReceiptsController(PampazonDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Receipt>>> GetAll()
        {
            return Ok(await _context.Receipts
                .Include(r => r.Client)
                .Include(r => r.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Receipt>> Get(int id)
        {
            var receipt = await _context.Receipts
                .Include(r => r.Client)
                .Include(r => r.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null)
                return NotFound();

            return Ok(receipt);
        }

        [HttpPost]
        public async Task<ActionResult<Receipt>> Create(Receipt receipt)
        {
            receipt.Date = DateTime.UtcNow;
            receipt.Status = ReceiptStatus.PendingEntry;

            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = receipt.Id }, receipt);
        }

        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ReceiptStatus newStatus)
        {
            var receipt = await _context.Receipts
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null)
                return NotFound();

            if (receipt.Status != ReceiptStatus.PendingEntry)
                return BadRequest("Can only update status of pending receipts");

            if (newStatus == ReceiptStatus.Entered)
            {
                // Validate all items have positions assigned
                if (!receipt.Items.Any())
                    return BadRequest("Cannot enter a receipt without items");

                receipt.Status = newStatus;
            }
            else if (newStatus == ReceiptStatus.Rejected)
            {
                receipt.Status = newStatus;
            }
            else
            {
                return BadRequest("Invalid status transition");
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/positions")]
        public async Task<IActionResult> AssignPositions(int id, [FromBody] List<StockPositionAssignment> assignments)
        {
            var receipt = await _context.Receipts
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null)
                return NotFound();

            if (receipt.Status != ReceiptStatus.PendingEntry)
                return BadRequest("Can only assign positions to pending receipts");

            // Get all positions that will be used
            var positions = await _context.StockPositions
                .Where(p => assignments.Select(a => a.PositionId).Contains(p.Id))
                .ToListAsync();

            if (positions.Count != assignments.Count)
                return BadRequest("Some positions were not found");

            // Update stock quantities
            foreach (var assignment in assignments)
            {
                var position = positions.First(p => p.Id == assignment.PositionId);
                var receiptItem = receipt.Items.FirstOrDefault(i => i.ProductId == position.ProductId);

                if (receiptItem == null)
                    return BadRequest($"Position {position.Id} contains a product not in the receipt");

                position.Quantity += assignment.Quantity;
            }

            receipt.Status = ReceiptStatus.Entered;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class StockPositionAssignment
    {
        public int PositionId { get; set; }
        public int Quantity { get; set; }
    }
} 