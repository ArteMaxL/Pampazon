using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Controllers
{
    /// <summary>
    /// Controlador para la gestión de despachos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DispatchesController : ControllerBase
    {
        private readonly PampazonDbContext _context;

        public DispatchesController(PampazonDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los despachos con sus órdenes asociadas
        /// </summary>
        /// <returns>Lista de despachos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Dispatch>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Dispatch>>> GetAll()
        {
            return Ok(await _context.Dispatches
                .Include(d => d.Order)
                    .ThenInclude(o => o.Client)
                .Include(d => d.Order)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(i => i.Product)
                .ToListAsync());
        }

        /// <summary>
        /// Obtiene un despacho específico por su número
        /// </summary>
        /// <param name="dispatchNumber">Número de despacho (formato: DISPxxxxxx)</param>
        /// <returns>Despacho solicitado</returns>
        [HttpGet("{dispatchNumber}")]
        [ProducesResponseType(typeof(Dispatch), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Dispatch>> Get(string dispatchNumber)
        {
            var dispatch = await _context.Dispatches
                .Include(d => d.Order)
                    .ThenInclude(o => o.Client)
                .Include(d => d.Order)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(d => d.DispatchNumber == dispatchNumber);

            if (dispatch == null)
                return NotFound();

            return Ok(dispatch);
        }

        /// <summary>
        /// Crea un nuevo despacho para una orden
        /// </summary>
        /// <param name="orderNumber">Número de orden a despachar</param>
        /// <returns>Despacho creado</returns>
        /// <response code="201">Despacho creado exitosamente</response>
        /// <response code="400">La orden no está lista para despacho</response>
        /// <response code="404">No se encontró la orden especificada</response>
        [HttpPost("orders/{orderNumber}")]
        [ProducesResponseType(typeof(Dispatch), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Dispatch>> CreateForOrder(string orderNumber)
        {
            var order = await _context.Orders.FindAsync(orderNumber);
            if (order == null)
                return NotFound();

            if (order.Status != OrderStatus.Prepared)
                return BadRequest("Order is not ready for dispatch");

            // Generate dispatch number
            var lastDispatch = await _context.Dispatches
                .OrderByDescending(d => d.DispatchNumber)
                .FirstOrDefaultAsync();

            int counter = 1;
            if (lastDispatch != null && int.TryParse(lastDispatch.DispatchNumber[4..], out int lastNumber))
            {
                counter = lastNumber + 1;
            }

            var dispatch = new Dispatch
            {
                DispatchNumber = $"DISP{counter:D6}",
                OrderNumber = orderNumber,
                Status = DispatchStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            order.Status = OrderStatus.Dispatched;

            _context.Dispatches.Add(dispatch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { dispatchNumber = dispatch.DispatchNumber }, dispatch);
        }

        /// <summary>
        /// Actualiza el estado de un despacho
        /// </summary>
        /// <param name="dispatchNumber">Número de despacho</param>
        /// <param name="newStatus">Nuevo estado del despacho</param>
        /// <returns>No content si la actualización es exitosa</returns>
        /// <response code="204">Estado actualizado exitosamente</response>
        /// <response code="400">El cambio de estado no es válido</response>
        /// <response code="404">No se encontró el despacho especificado</response>
        [HttpPost("{dispatchNumber}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(string dispatchNumber, [FromBody] DispatchStatus newStatus)
        {
            var dispatch = await _context.Dispatches.FindAsync(dispatchNumber);
            if (dispatch == null)
                return NotFound();

            if (dispatch.Status == DispatchStatus.Delivered)
                return BadRequest("Cannot update status of delivered dispatches");

            if (newStatus == DispatchStatus.Delivered && dispatch.Status != DispatchStatus.InTransit)
                return BadRequest("Can only mark in-transit dispatches as delivered");

            dispatch.Status = newStatus;
            if (newStatus == DispatchStatus.Delivered)
                dispatch.DeliveredAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
} 