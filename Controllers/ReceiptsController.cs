using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using Pampazon.Models;
using Pampazon.Enums;
using Pampazon.Services;

namespace Pampazon.Controllers;

/// <summary>
/// Controlador para la gestión de recibos de mercadería
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _service;
    public ReceiptsController(IReceiptService service) => _service = service;
    
    /// <summary>
    /// Obtiene todos los recibos de mercadería con sus detalles
    /// </summary>
    /// <returns>Lista de recibos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Receipt>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Receipt>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
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
        var receipt = await _service.GetAsync(receiptNumber);
        if (receipt == null)
            return Problem(title: "No encontrado", detail: "No se encontró el recibo especificado", statusCode: StatusCodes.Status404NotFound);
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
        try
        {
            var created = await _service.CreateAsync(receipt);
            return CreatedAtAction(nameof(Get), new { receiptNumber = created.ReceiptNumber }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Datos inválidos", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
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
        try
        {
            await _service.UpdateStatusAsync(receiptNumber, newStatus);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Problem(title: "No encontrado", detail: "No se encontró el recibo especificado", statusCode: StatusCodes.Status404NotFound);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Datos inválidos", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
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
        try
        {
            await _service.AssignLocationAsync(receiptNumber, productId, location);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Problem(title: "No encontrado", detail: "No se encontró el recibo o el producto especificado", statusCode: StatusCodes.Status404NotFound);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Datos inválidos", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
