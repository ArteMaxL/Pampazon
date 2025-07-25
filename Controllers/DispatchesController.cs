using Microsoft.AspNetCore.Mvc;
using Pampazon.Models;
using Pampazon.Enums;
using Pampazon.Services;

namespace Pampazon.Controllers;

/// <summary>
/// Controlador para la gestión de despachos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DispatchesController : ControllerBase
{
    private readonly IDispatchService _service;
    public DispatchesController(IDispatchService service) => _service = service;

    /// <summary>
    /// Obtiene todos los despachos con sus órdenes asociadas
    /// </summary>
    /// <returns>Lista de despachos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Dispatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Dispatch>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
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
        var dispatch = await _service.GetAsync(dispatchNumber);
        if (dispatch == null)
            return Problem(title: "No encontrado", detail: "No se encontró el despacho especificado", statusCode: StatusCodes.Status404NotFound);
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
        try
        {
            var dispatch = await _service.CreateForOrderAsync(orderNumber);
            return CreatedAtAction(nameof(Get), new { dispatchNumber = dispatch.DispatchNumber }, dispatch);
        }
        catch (KeyNotFoundException)
        {
            return Problem(title: "No encontrado", detail: "No se encontró la orden especificada", statusCode: StatusCodes.Status404NotFound);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Datos inválidos", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
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
        try
        {
            await _service.UpdateStatusAsync(dispatchNumber, newStatus);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Problem(title: "No encontrado", detail: "No se encontró el despacho especificado", statusCode: StatusCodes.Status404NotFound);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Datos inválidos", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
