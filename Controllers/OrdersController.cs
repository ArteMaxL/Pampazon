using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly PampazonDbContext _context;

        public OrdersController(PampazonDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            return Ok(await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync());
        }

        [HttpGet("{orderNumber}")]
        public async Task<ActionResult<Order>> Get(string orderNumber)
        {
            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Create(Order order)
        {
            // Generate order number
            var lastOrder = await _context.Orders
                .OrderByDescending(o => o.OrderNumber)
                .FirstOrDefaultAsync();

            int counter = 1;
            if (lastOrder != null && int.TryParse(lastOrder.OrderNumber[3..], out int lastNumber))
            {
                counter = lastNumber + 1;
            }

            order.OrderNumber = $"ORD{counter:D6}";
            order.Date = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { orderNumber = order.OrderNumber }, order);
        }

        [HttpPost("{orderNumber}/status")]
        public async Task<IActionResult> UpdateStatus(string orderNumber, [FromBody] OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderNumber);
            if (order == null)
                return NotFound();

            if (order.Status != OrderStatus.Pending && newStatus == OrderStatus.Prepared)
                return BadRequest("Can only prepare pending orders");

            if (order.Status != OrderStatus.Prepared && newStatus == OrderStatus.Dispatched)
                return BadRequest("Can only dispatch prepared orders");

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{orderNumber}/positions")]
        public async Task<IActionResult> AssignPositions(string orderNumber, [FromBody] List<int> positionIds)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                return NotFound();

            if (order.Status != OrderStatus.Pending)
                return BadRequest("Can only assign positions to pending orders");

            var positions = await _context.StockPositions
                .Include(p => p.Product)
                .Where(p => positionIds.Contains(p.Id))
                .ToListAsync();

            if (positions.Count != positionIds.Count)
                return BadRequest("Some positions were not found");

            // Validate that positions have enough stock for the order
            foreach (var item in order.Items)
            {
                var availableStock = positions
                    .Where(p => p.ProductId == item.ProductId)
                    .Sum(p => p.Quantity);

                if (availableStock < item.Quantity)
                    return BadRequest($"Not enough stock for product {item.ProductId}");
            }

            // Update stock quantities
            foreach (var item in order.Items)
            {
                var remainingQuantity = item.Quantity;
                var productPositions = positions
                    .Where(p => p.ProductId == item.ProductId)
                    .OrderBy(p => p.Quantity);

                foreach (var position in productPositions)
                {
                    if (remainingQuantity <= 0) break;

                    var quantityToTake = Math.Min(remainingQuantity, position.Quantity);
                    position.Quantity -= quantityToTake;
                    remainingQuantity -= quantityToTake;
                }
            }

            order.Status = OrderStatus.Prepared;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
} 