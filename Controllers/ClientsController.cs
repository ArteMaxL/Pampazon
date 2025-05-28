using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly PampazonDbContext _context;

        public ClientsController(PampazonDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAll()
        {
            return Ok(await _context.Clients.ToListAsync());
        }

        [HttpGet("{cuit}")]
        public async Task<ActionResult<Client>> Get(string cuit)
        {
            var client = await _context.Clients.FindAsync(cuit);
            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<Client>> Create(Client client)
        {
            if (await _context.Clients.AnyAsync(c => c.CUIT == client.CUIT))
                return Conflict("A client with this CUIT already exists");

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { cuit = client.CUIT }, client);
        }

        [HttpPut("{cuit}")]
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

        [HttpDelete("{cuit}")]
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