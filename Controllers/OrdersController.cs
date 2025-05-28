using Microsoft.AspNetCore.Mvc;
using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private static readonly List<Order> _orders = new();
        private static int _orderCounter = 0;

        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetAll()
        {
            return Ok(_orders);
        }

        [HttpGet("{orderNumber}")]
        public ActionResult<Order> Get(string orderNumber)
        {
            var order = _orders.FirstOrDefault(o => o.OrderNumber == orderNumber);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPut]
        public ActionResult<Order> Create(Order order)
        {
            order.OrderNumber = $"ORD{++_orderCounter:D6}";
            order.Date = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            _orders.Add(order);
            return CreatedAtAction(nameof(Get), new { orderNumber = order.OrderNumber }, order);
        }

        [HttpPost("{orderNumber}/status")]
        public IActionResult UpdateStatus(string orderNumber, [FromBody] OrderStatus newStatus)
        {
            var order = _orders.FirstOrDefault(o => o.OrderNumber == orderNumber);
            if (order == null)
                return NotFound();

            if (order.Status != OrderStatus.Pending && newStatus == OrderStatus.Prepared)
                return BadRequest("Can only prepare pending orders");

            if (order.Status != OrderStatus.Prepared && newStatus == OrderStatus.Dispatched)
                return BadRequest("Can only dispatch prepared orders");

            order.Status = newStatus;
            return NoContent();
        }

        [HttpPost("{orderNumber}/positions")]
        public IActionResult AssignPositions(string orderNumber, [FromBody] List<StockPosition> positions)
        {
            var order = _orders.FirstOrDefault(o => o.OrderNumber == orderNumber);
            if (order == null)
                return NotFound();

            if (order.Status != OrderStatus.Pending)
                return BadRequest("Can only assign positions to pending orders");

            // Here we would validate positions and update stock
            // This is simplified for now

            order.Status = OrderStatus.Prepared;
            return NoContent();
        }
    }
} 