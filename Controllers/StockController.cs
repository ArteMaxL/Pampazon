using Microsoft.AspNetCore.Mvc;
using Pampazon.Models;
using Pampazon.Services;

namespace Pampazon.Controllers;

/// <summary>
/// Controlador para la gestión del stock en almacén
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IStockService _service;
    public StockController(IStockService service) => _service = service;

    /// <summary>
    /// Obtiene posiciones de stock paginadas, filtradas y ordenadas
    /// </summary>
    /// <param name="page">Página (por defecto 1)</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10)</param>
    /// <param name="search">Filtro por producto o cliente</param>
    /// <param name="orderBy">Campo de ordenamiento (productId, clientId, quantity)</param>
    /// <param name="desc">Orden descendente</param>
    /// <returns>Página de posiciones de stock</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<StockPosition>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<StockPosition>>> GetAll(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        string? orderBy = null,
        bool desc = false)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search, orderBy, desc);
        return Ok(result);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<StockPosition>>> GetByProduct(string productId)
    {
        return Ok(await _service.GetByProductAsync(productId));
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
        try
        {
            var created = await _service.CreateAsync(position);
            return CreatedAtAction(nameof(GetByProduct), new { productId = created.ProductId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Datos inválidos", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (ArgumentException ex)
        {
            return Problem(title: "Conflicto", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
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
        try
        {
            await _service.UpdateQuantityAsync(id, quantity);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Problem(title: "No encontrado", detail: "No se encontró la posición de stock especificada", statusCode: StatusCodes.Status404NotFound);
        }
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
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Problem(title: "No encontrado", detail: "No se encontró la posición de stock especificada", statusCode: StatusCodes.Status404NotFound);
        }
    }
}
