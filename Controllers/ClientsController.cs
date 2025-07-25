using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;

namespace Pampazon.Controllers;

/// <summary>
/// Controlador para la gestión de clientes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClientsController(PampazonDbContext context) : ControllerBase
{
    /// <summary>
    /// Obtiene todos los clientes registrados
    /// </summary>
    /// <returns>Lista de clientes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Client>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Client>>> GetAll()
    {
        return Ok(await context.Clients.ToListAsync());
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
        var client = await context.Clients.FindAsync(cuit);
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
        if (await context.Clients.AnyAsync(c => c.CUIT == client.CUIT))
            return Problem(title: "Conflicto", detail: "Ya existe un cliente con el CUIT especificado", statusCode: StatusCodes.Status409Conflict);
        context.Clients.Add(client);
        await context.SaveChangesAsync();
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
        var existingClient = await context.Clients.FindAsync(cuit);
        if (existingClient == null)
            return Problem(title: "No encontrado", detail: "No se encontró el cliente especificado", statusCode: StatusCodes.Status404NotFound);
        context.Entry(existingClient).CurrentValues.SetValues(client);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ClientExists(cuit))
                return Problem(title: "No encontrado", detail: "No se encontró el cliente especificado", statusCode: StatusCodes.Status404NotFound);
            throw;
        }
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
        var client = await context.Clients.FindAsync(cuit);
        if (client == null)
            return Problem(title: "No encontrado", detail: "No se encontró el cliente especificado", statusCode: StatusCodes.Status404NotFound);
        context.Clients.Remove(client);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<bool> ClientExists(string cuit)
    {
        return await context.Clients.AnyAsync(c => c.CUIT == cuit);
    }
}
