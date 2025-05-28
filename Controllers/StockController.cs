using Microsoft.AspNetCore.Mvc;
using Pampazon.Models;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private static readonly List<StockPosition> _positions = new();

        [HttpGet]
        public ActionResult<IEnumerable<StockPosition>> GetAll()
        {
            return Ok(_positions);
        }

        [HttpGet("product/{productCode}")]
        public ActionResult<IEnumerable<StockPosition>> GetByProduct(string productCode)
        {
            var positions = _positions.Where(p => p.ProductCode == productCode);
            return Ok(positions);
        }

        [HttpPost]
        public ActionResult<StockPosition> Create(StockPosition position)
        {
            // Validate position doesn't exist
            if (_positions.Any(p => 
                p.Aisle == position.Aisle && 
                p.Section == position.Section && 
                p.Shelf == position.Shelf && 
                p.Level == position.Level))
            {
                return Conflict("This position is already in use");
            }

            position.Id = _positions.Count > 0 ? _positions.Max(p => p.Id) + 1 : 1;
            _positions.Add(position);

            return CreatedAtAction(nameof(GetByProduct), new { productCode = position.ProductCode }, position);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, StockPosition position)
        {
            if (id != position.Id)
                return BadRequest();

            var existingPosition = _positions.FirstOrDefault(p => p.Id == id);
            if (existingPosition == null)
                return NotFound();

            // Update quantity and other details
            var index = _positions.IndexOf(existingPosition);
            _positions[index] = position;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var position = _positions.FirstOrDefault(p => p.Id == id);
            if (position == null)
                return NotFound();

            _positions.Remove(position);
            return NoContent();
        }
    }
} 