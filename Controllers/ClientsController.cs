using Microsoft.AspNetCore.Mvc;
using Pampazon.Models;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private static readonly List<Client> _clients = new();

        [HttpGet]
        public ActionResult<IEnumerable<Client>> GetAll()
        {
            return Ok(_clients);
        }

        [HttpGet("{cuit}")]
        public ActionResult<Client> Get(string cuit)
        {
            var client = _clients.FirstOrDefault(c => c.CUIT == cuit);
            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public ActionResult<Client> Create(Client client)
        {
            if (_clients.Any(c => c.CUIT == client.CUIT))
                return Conflict("A client with this CUIT already exists");

            _clients.Add(client);
            return CreatedAtAction(nameof(Get), new { cuit = client.CUIT }, client);
        }

        [HttpPut("{cuit}")]
        public IActionResult Update(string cuit, Client client)
        {
            if (cuit != client.CUIT)
                return BadRequest();

            var existingClient = _clients.FirstOrDefault(c => c.CUIT == cuit);
            if (existingClient == null)
                return NotFound();

            var index = _clients.IndexOf(existingClient);
            _clients[index] = client;

            return NoContent();
        }

        [HttpDelete("{cuit}")]
        public IActionResult Delete(string cuit)
        {
            var client = _clients.FirstOrDefault(c => c.CUIT == cuit);
            if (client == null)
                return NotFound();

            _clients.Remove(client);
            return NoContent();
        }
    }
} 