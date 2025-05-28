using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;

namespace Pampazon.Controllers
{
    /// <summary>
    /// Controlador para la gestión de clientes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly PampazonDbContext _context;

        public ClientsController(PampazonDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los clientes registrados
        /// </summary>
        /// <returns>Lista de clientes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Client>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Client>>> GetAll()
        {
            return Ok(await _context.Clients.ToListAsync());
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
            var client = await _context.Clients.FindAsync(cuit);
            if (client == null)
                return NotFound();

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
            if (await _context.Clients.AnyAsync(c => c.CUIT == client.CUIT))
                return Conflict("A client with this CUIT already exists");

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

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
                return BadRequest();

            var existingClient = await _context.Clients.FindAsync(cuit);
            if (existingClient == null)
                return NotFound();

            _context.Entry(existingClient).CurrentValues.SetValues(client);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ClientExists(cuit))
                    return NotFound();
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
            var client = await _context.Clients.FindAsync(cuit);
            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> ClientExists(string cuit)
        {
            return await _context.Clients.AnyAsync(c => c.CUIT == cuit);
        }
    }
} 