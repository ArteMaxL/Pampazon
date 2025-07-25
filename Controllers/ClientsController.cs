using Microsoft.AspNetCore.Mvc;
using Pampazon.Models;
using Pampazon.Services;

namespace Pampazon.Controllers;

/// <summary>
/// Controlador para la gestión de clientes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _service;
    public ClientsController(IClientService service) => _service = service;

    /// <summary>
    /// Obtiene clientes paginados, filtrados y ordenados
    /// </summary>
    /// <param name="page">Página (por defecto 1)</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10)</param>
    /// <param name="search">Filtro por razón social</param>
    /// <param name="orderBy">Campo de ordenamiento (cuit, businessName)</param>
    /// <param name="desc">Orden descendente</param>
    /// <returns>Página de clientes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Client>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<Client>>> GetAll(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        string? orderBy = null,
        bool desc = false)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search, orderBy, desc);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene un cliente específico por su CUIT
    /// </summary>
    /// <param name="cuit">CUIT del cliente</param>
    /// <returns>Cliente solicitado</returns>
    [HttpGet("{cuit}")]
    [ProducesResponseType(typeof(Client), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Client>> Get(string cuit)
    {
        var client = await _service.GetAsync(cuit);

        if (client == null)
            return Problem(title: "No encontrado", detail: "No se encontró el cliente especificado", statusCode: StatusCodes.Status404NotFound);
        
        return Ok(client);
    }

    /// <summary>
    /// Registra un nuevo cliente
    /// </summary>
    /// <param name="client">Datos del cliente a registrar</param>
    /// <returns>Cliente creado</returns>
    /// <response code="201">Cliente creado exitosamente</response>
    /// <response code="409">Ya existe un cliente con el CUIT especificado</response>
    [HttpPost]
    [ProducesResponseType(typeof(Client), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Client>> Create(Client client)
    {
        if ((await _service.GetAsync(client.CUIT)) != null)
            return Problem(title: "Conflicto", detail: "Ya existe un cliente con el CUIT especificado", statusCode: StatusCodes.Status409Conflict);
        
        await _service.CreateAsync(client);
        return CreatedAtAction(nameof(Get), new { cuit = client.CUIT }, client);
    }

    /// <summary>
    /// Actualiza los datos de un cliente existente
    /// </summary>
    /// <param name="cuit">CUIT del cliente a actualizar</param>
    /// <param name="client">Nuevos datos del cliente</param>
    /// <returns>No content si la actualización es exitosa</returns>
    /// <response code="204">Cliente actualizado exitosamente</response>
    /// <response code="400">El CUIT en la URL no coincide con el del cliente</response>
    /// <response code="404">No se encontró el cliente especificado</response>
    [HttpPut("{cuit}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string cuit, Client client)
    {
        if (cuit != client.CUIT)
            return Problem(title: "Datos inválidos", detail: "El CUIT en la URL no coincide con el del cliente", statusCode: StatusCodes.Status400BadRequest);
        
        var existingClient = await _service.GetAsync(cuit);
        if (existingClient == null)
            return Problem(title: "No encontrado", detail: "No se encontró el cliente especificado", statusCode: StatusCodes.Status404NotFound);
        
        await _service.UpdateAsync(cuit, client);
        return NoContent();
    }

    /// <summary>
    /// Elimina un cliente
    /// </summary>
    /// <param name="cuit">CUIT del cliente a eliminar</param>
    /// <returns>No content si la eliminación es exitosa</returns>
    /// <response code="204">Cliente eliminado exitosamente</response>
    /// <response code="404">No se encontró el cliente especificado</response>
    [HttpDelete("{cuit}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string cuit)
    {
        var client = await _service.GetAsync(cuit);
        if (client == null)
            return Problem(title: "No encontrado", detail: "No se encontró el cliente especificado", statusCode: StatusCodes.Status404NotFound);
        
        await _service.DeleteAsync(cuit);
        return NoContent();
    }
}
