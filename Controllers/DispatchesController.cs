using Microsoft.AspNetCore.Mvc;
using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DispatchesController : ControllerBase
    {
        private static readonly List<Dispatch> _dispatches = new();
        private static int _dispatchCounter = 0;

        [HttpGet]
        public ActionResult<IEnumerable<Dispatch>> GetAll()
        {
            return Ok(_dispatches);
        }

        [HttpGet("{dispatchNumber}")]
        public ActionResult<Dispatch> Get(string dispatchNumber)
        {
            var dispatch = _dispatches.FirstOrDefault(d => d.DispatchNumber == dispatchNumber);
            if (dispatch == null)
                return NotFound();

            return Ok(dispatch);
        }

        [HttpPut]
        public ActionResult<Dispatch> Create(Dispatch dispatch)
        {
            dispatch.DispatchNumber = $"DSP{++_dispatchCounter:D6}";
            dispatch.Date = DateTime.UtcNow;
            dispatch.IsFinalized = false;

            _dispatches.Add(dispatch);
            return CreatedAtAction(nameof(Get), new { dispatchNumber = dispatch.DispatchNumber }, dispatch);
        }

        [HttpPost("{dispatchNumber}/orders/{orderNumber}")]
        public IActionResult AddOrder(string dispatchNumber, string orderNumber)
        {
            var dispatch = _dispatches.FirstOrDefault(d => d.DispatchNumber == dispatchNumber);
            if (dispatch == null)
                return NotFound("Dispatch not found");

            if (dispatch.IsFinalized)
                return BadRequest("Cannot modify a finalized dispatch");

            // Here we would get the order and validate its status
            // This is simplified for now
            
            return NoContent();
        }

        [HttpPost("{dispatchNumber}/finalize")]
        public IActionResult Finalize(string dispatchNumber)
        {
            var dispatch = _dispatches.FirstOrDefault(d => d.DispatchNumber == dispatchNumber);
            if (dispatch == null)
                return NotFound();

            if (dispatch.IsFinalized)
                return BadRequest("Dispatch is already finalized");

            if (!dispatch.Orders.Any())
                return BadRequest("Cannot finalize an empty dispatch");

            // Here we would update all orders' status to Dispatched
            // This is simplified for now

            dispatch.IsFinalized = true;
            return NoContent();
        }
    }
} 