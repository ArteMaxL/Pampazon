using Microsoft.AspNetCore.Mvc;
using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptsController : ControllerBase
    {
        private static readonly List<Receipt> _receipts = new();

        [HttpGet]
        public ActionResult<IEnumerable<Receipt>> GetAll()
        {
            return Ok(_receipts);
        }

        [HttpGet("{id}")]
        public ActionResult<Receipt> Get(int id)
        {
            var receipt = _receipts.FirstOrDefault(r => r.Id == id);
            if (receipt == null)
                return NotFound();

            return Ok(receipt);
        }

        [HttpPut]
        public ActionResult<Receipt> Create(Receipt receipt)
        {
            receipt.Id = _receipts.Count > 0 ? _receipts.Max(r => r.Id) + 1 : 1;
            receipt.Date = DateTime.UtcNow;
            receipt.Status = ReceiptStatus.PendingEntry;

            _receipts.Add(receipt);
            return CreatedAtAction(nameof(Get), new { id = receipt.Id }, receipt);
        }

        [HttpPost("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] ReceiptStatus newStatus)
        {
            var receipt = _receipts.FirstOrDefault(r => r.Id == id);
            if (receipt == null)
                return NotFound();

            if (receipt.Status != ReceiptStatus.PendingEntry)
                return BadRequest("Can only update status of pending receipts");

            if (newStatus == ReceiptStatus.Entered)
            {
                // Here we would update stock positions
                // This is simplified for now
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

            return NoContent();
        }

        [HttpPost("{id}/positions")]
        public IActionResult AssignPositions(int id, [FromBody] List<StockPosition> positions)
        {
            var receipt = _receipts.FirstOrDefault(r => r.Id == id);
            if (receipt == null)
                return NotFound();

            if (receipt.Status != ReceiptStatus.PendingEntry)
                return BadRequest("Can only assign positions to pending receipts");

            // Here we would validate positions and update stock
            // This is simplified for now

            receipt.Status = ReceiptStatus.Entered;
            return NoContent();
        }
    }
} 