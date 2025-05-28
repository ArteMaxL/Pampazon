using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;

namespace Pampazon.Controllers;

/// <summary>
/// Controlador para la gestión del stock en almacén
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StockController(PampazonDbContext context) : ControllerBase
{

    /// <summary>
    /// Obtiene todas las posiciones de stock con sus productos
    /// </summary>
    /// <returns>Lista de posiciones de stock</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StockPosition>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StockPosition>>> GetAll()
    {
        var positions = await context.StockPositions
            .Include(p => p.Product)
            .Include(p => p.Client)
            .ToListAsync();
        return Ok(positions);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<StockPosition>>> GetByProduct(string productId)
    {
        var positions = await context.StockPositions
            .Include(p => p.Product)
            .Include(p => p.Client)
            .Where(p => p.ProductId == productId)
            .ToListAsync();
        return Ok(positions);
    }

    /// <summary>
    /// Registra una nueva posición de stock
    /// </summary>
    /// <param name="position">Datos de la posición de stock</param>
    /// <returns>Posición de stock creada</returns>
    /// <response code="201">Posición de stock creada exitosamente</response>
    /// <response code="400">Datos inválidos o referencias no existentes</response>
    /// <response code="409">La posición ya está en uso</response>
    [HttpPost]
    [ProducesResponseType(typeof(StockPosition), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StockPosition>> Create(StockPosition position)
    {
        // Validate product exists
        var productExists = await context.Products.AnyAsync(p => p.Code == position.ProductId);
        if (!productExists)
        {
            return BadRequest($"El producto con código {position.ProductId} no existe");
        }

        // Validate client exists
        var clientExists = await context.Clients.AnyAsync(c => c.CUIT == position.ClientId);
        if (!clientExists)
        {
            return BadRequest($"El cliente con CUIT {position.ClientId} no existe");
        }

        // Validate receipt exists
        var receiptExists = await context.Receipts.AnyAsync(r => r.ReceiptNumber == position.ReceiptNumber);
        if (!receiptExists)
        {
            return BadRequest($"El recibo {position.ReceiptNumber} no existe");
        }

        // Validate position doesn't exist
        var exists = await context.StockPositions.AnyAsync(p => 
            p.Aisle == position.Aisle && 
            p.Section == position.Section && 
            p.Shelf == position.Shelf && 
            p.Level == position.Level);

        if (exists)
        {
            return Conflict("Esta posición ya está en uso");
        }

        // Set creation date
        position.CreatedAt = DateTime.UtcNow;

        context.StockPositions.Add(position);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByProduct), new { productId = position.ProductId }, position);
    }

    /// <summary>
    /// Actualiza la cantidad en una posición de stock
    /// </summary>
    /// <param name="id">ID de la posición de stock</param>
    /// <param name="quantity">Nueva cantidad</param>
    /// <returns>No content si la actualización es exitosa</returns>
    /// <response code="204">Cantidad actualizada exitosamente</response>
    /// <response code="404">No se encontró la posición de stock especificada</response>
    [HttpPut("{id}/quantity")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuantity(int id, [FromBody] int quantity)
    {
        var position = await context.StockPositions.FindAsync(id);
        if (position == null)
            return NotFound();

        position.Quantity = quantity;
        await context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Elimina una posición de stock
    /// </summary>
    /// <param name="id">ID de la posición de stock</param>
    /// <returns>No content si la eliminación es exitosa</returns>
    /// <response code="204">Posición de stock eliminada exitosamente</response>
    /// <response code="404">No se encontró la posición de stock especificada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var position = await context.StockPositions.FindAsync(id);
        if (position == null)
            return NotFound();

        context.StockPositions.Remove(position);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<bool> StockPositionExists(int id)
    {
        return await context.StockPositions.AnyAsync(p => p.Id == id);
    }
}
