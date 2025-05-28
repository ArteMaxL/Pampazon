using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using Pampazon.Enums;
using System.ComponentModel.DataAnnotations;

namespace Pampazon.Controllers;

/// <summary>
/// Controlador para la gestión de órdenes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController(PampazonDbContext context) : ControllerBase
{

    /// <summary>
    /// Obtiene todas las órdenes con sus items y productos asociados
    /// </summary>
    /// <returns>Lista de órdenes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll()
    {
        return Ok(await context.Orders
            .Include(o => o.Client)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .ToListAsync());
    }

    /// <summary>
    /// Obtiene una orden específica por su número
    /// </summary>
    /// <param name="orderNumber">Número de orden (formato: ORDxxxxxx)</param>
    /// <returns>Orden solicitada</returns>
    [HttpGet("{orderNumber}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order>> Get(string orderNumber)
    {
        var order = await context.Orders
            .Include(o => o.Client)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    /// <summary>
    /// Crea una nueva orden
    /// </summary>
    /// <param name="order">Datos de la orden</param>
    /// <returns>Orden creada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Order>> Create(Order order)
    {
        // Generate order number
        var lastOrder = await context.Orders
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

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { orderNumber = order.OrderNumber }, order);
    }

    /// <summary>
    /// Actualiza el estado de una orden
    /// </summary>
    /// <param name="orderNumber">Número de orden</param>
    /// <param name="newStatus">Nuevo estado</param>
    /// <returns>No content si la actualización es exitosa</returns>
    [HttpPost("{orderNumber}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(string orderNumber, [FromBody][Required] OrderStatus newStatus)
    {
        var order = await context.Orders.FindAsync(orderNumber);
        if (order == null)
            return NotFound();

        if (order.Status != OrderStatus.Pending && newStatus == OrderStatus.Prepared)
            return BadRequest("Can only prepare pending orders");

        if (order.Status != OrderStatus.Prepared && newStatus == OrderStatus.Dispatched)
            return BadRequest("Can only dispatch prepared orders");

        order.Status = newStatus;
        await context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Asigna posiciones de stock a una orden y la marca como preparada
    /// </summary>
    /// <param name="orderNumber">Número de orden</param>
    /// <param name="positionIds">Lista de IDs de posiciones de stock</param>
    /// <returns>No content si la asignación es exitosa</returns>
    [HttpPost("{orderNumber}/positions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignPositions(string orderNumber, [FromBody][Required] List<int> positionIds)
    {
        var order = await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        if (order == null)
            return NotFound();

        if (order.Status != OrderStatus.Pending)
            return BadRequest("Can only assign positions to pending orders");

        var positions = await context.StockPositions
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
        await context.SaveChangesAsync();

        return NoContent();
    }
}
