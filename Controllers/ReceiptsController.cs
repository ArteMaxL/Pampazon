using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Controllers;

/// <summary>
/// Controlador para la gestión de recibos de mercadería
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReceiptsController(PampazonDbContext context) : ControllerBase
{

    /// <summary>
    /// Obtiene todos los recibos de mercadería con sus detalles
    /// </summary>
    /// <returns>Lista de recibos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Receipt>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Receipt>>> GetAll()
    {
        return Ok(await context.Receipts
            .Include(r => r.Client)
            .Include(r => r.Items)
                .ThenInclude(i => i.Product)
            .ToListAsync());
    }

    /// <summary>
    /// Obtiene un recibo específico por su número
    /// </summary>
    /// <param name="receiptNumber">Número de recibo (formato: RCPxxxxxx)</param>
    /// <returns>Recibo solicitado</returns>
    [HttpGet("{receiptNumber}")]
    [ProducesResponseType(typeof(Receipt), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Receipt>> Get(string receiptNumber)
    {
        var receipt = await context.Receipts
            .Include(r => r.Client)
            .Include(r => r.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(r => r.ReceiptNumber == receiptNumber);

        if (receipt == null)
            return NotFound();

        return Ok(receipt);
    }

    /// <summary>
    /// Crea un nuevo recibo de mercadería
    /// </summary>
    /// <param name="receipt">Datos del recibo a crear</param>
    /// <returns>Recibo creado</returns>
    /// <response code="201">Recibo creado exitosamente</response>
    /// <response code="400">Datos del recibo inválidos o cliente no encontrado</response>
    [HttpPost]
    [ProducesResponseType(typeof(Receipt), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Receipt>> Create(Receipt receipt)
    {
        // Validate client exists
        var client = await context.Clients.FindAsync(receipt.ClientId);
        if (client == null)
            return BadRequest("Client not found");

        // Validate products exist
        foreach (var item in receipt.Items)
        {
            var product = await context.Products.FindAsync(item.ProductId);
            if (product == null)
                return BadRequest($"Product {item.ProductId} not found");
        }

        // Generate receipt number
        var lastReceipt = await context.Receipts
            .OrderByDescending(r => r.ReceiptNumber)
            .FirstOrDefaultAsync();

        int counter = 1;
        if (lastReceipt != null && int.TryParse(lastReceipt.ReceiptNumber[3..], out int lastNumber))
        {
            counter = lastNumber + 1;
        }

        receipt.ReceiptNumber = $"RCP{counter:D6}";
        receipt.Date = DateTime.UtcNow;
        receipt.Status = ReceiptStatus.Pending;

        context.Receipts.Add(receipt);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { receiptNumber = receipt.ReceiptNumber }, receipt);
    }

    /// <summary>
    /// Actualiza el estado de un recibo
    /// </summary>
    /// <param name="receiptNumber">Número de recibo</param>
    /// <param name="newStatus">Nuevo estado del recibo</param>
    /// <returns>No content si la actualización es exitosa</returns>
    /// <response code="204">Estado actualizado exitosamente</response>
    /// <response code="400">El cambio de estado no es válido</response>
    /// <response code="404">No se encontró el recibo especificado</response>
    [HttpPost("{receiptNumber}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(string receiptNumber, [FromBody] ReceiptStatus newStatus)
    {
        var receipt = await context.Receipts.FindAsync(receiptNumber);
        if (receipt == null)
            return NotFound();

        if (receipt.Status == ReceiptStatus.Completed)
            return BadRequest("Cannot update status of completed receipts");

        if (newStatus == ReceiptStatus.Completed && receipt.Status != ReceiptStatus.InProgress)
            return BadRequest("Can only complete receipts that are in progress");

        receipt.Status = newStatus;
        if (newStatus == ReceiptStatus.Completed)
        {
            receipt.CompletedAt = DateTime.UtcNow;

            // Update stock positions
            foreach (var item in receipt.Items)
            {
                var stockPosition = new StockPosition
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ClientId = receipt.ClientId,
                    ReceiptNumber = receipt.ReceiptNumber,
                    CreatedAt = DateTime.UtcNow
                };

                context.StockPositions.Add(stockPosition);
            }
        }

        await context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Asigna una ubicación a una posición de stock de un recibo
    /// </summary>
    /// <param name="receiptNumber">Número de recibo</param>
    /// <param name="productId">ID del producto</param>
    /// <param name="location">Datos de la ubicación</param>
    /// <returns>No content si la asignación es exitosa</returns>
    /// <response code="204">Ubicación asignada exitosamente</response>
    /// <response code="400">El recibo no está en progreso o la ubicación ya está ocupada</response>
    /// <response code="404">No se encontró el recibo o el producto especificado</response>
    [HttpPost("{receiptNumber}/items/{productId}/location")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignLocation(string receiptNumber, string productId, [FromBody] StockLocation location)
    {
        var receipt = await context.Receipts
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.ReceiptNumber == receiptNumber);

        if (receipt == null)
            return NotFound("Receipt not found");

        if (receipt.Status != ReceiptStatus.InProgress)
            return BadRequest("Can only assign locations to receipts in progress");

        var item = receipt.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            return NotFound("Product not found in receipt");

        // Validate location is not already in use
        var exists = await context.StockPositions.AnyAsync(p =>
            p.Aisle == location.Aisle &&
            p.Section == location.Section &&
            p.Shelf == location.Shelf &&
            p.Level == location.Level);

        if (exists)
            return BadRequest("Location is already in use");

        // Update or create stock position
        var stockPosition = await context.StockPositions
            .FirstOrDefaultAsync(p => p.ReceiptNumber == receiptNumber && p.ProductId == productId);

        if (stockPosition == null)
        {
            stockPosition = new StockPosition
            {
                ProductId = productId,
                Quantity = item.Quantity,
                ClientId = receipt.ClientId,
                ReceiptNumber = receiptNumber,
                CreatedAt = DateTime.UtcNow
            };
            context.StockPositions.Add(stockPosition);
        }

        stockPosition.Aisle = location.Aisle;
        stockPosition.Section = location.Section;
        stockPosition.Shelf = location.Shelf;
        stockPosition.Level = location.Level;

        await context.SaveChangesAsync();
        return NoContent();
    }
}
