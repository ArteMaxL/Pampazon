using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly PampazonDbContext _context;

        public ProductsController(PampazonDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            return Ok(await _context.Products.ToListAsync());
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<Product>> Get(string code)
        {
            var product = await _context.Products.FindAsync(code);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            if (await _context.Products.AnyAsync(p => p.Code == product.Code))
                return Conflict("A product with this code already exists");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { code = product.Code }, product);
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> Update(string code, Product product)
        {
            if (code != product.Code)
                return BadRequest();

            var existingProduct = await _context.Products.FindAsync(code);
            if (existingProduct == null)
                return NotFound();

            _context.Entry(existingProduct).CurrentValues.SetValues(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProductExists(code))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            var product = await _context.Products.FindAsync(code);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> ProductExists(string code)
        {
            return await _context.Products.AnyAsync(p => p.Code == code);
        }
    }
} 