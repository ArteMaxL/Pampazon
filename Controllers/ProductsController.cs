using Microsoft.AspNetCore.Mvc;
using Pampazon.Models;

namespace Pampazon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<Product> _products = new();

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAll()
        {
            return Ok(_products);
        }

        [HttpGet("{code}")]
        public ActionResult<Product> Get(string code)
        {
            var product = _products.FirstOrDefault(p => p.Code == code);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> Create(Product product)
        {
            if (_products.Any(p => p.Code == product.Code))
                return Conflict("A product with this code already exists");

            _products.Add(product);
            return CreatedAtAction(nameof(Get), new { code = product.Code }, product);
        }

        [HttpPut("{code}")]
        public IActionResult Update(string code, Product product)
        {
            if (code != product.Code)
                return BadRequest();

            var existingProduct = _products.FirstOrDefault(p => p.Code == code);
            if (existingProduct == null)
                return NotFound();

            var index = _products.IndexOf(existingProduct);
            _products[index] = product;

            return NoContent();
        }

        [HttpDelete("{code}")]
        public IActionResult Delete(string code)
        {
            var product = _products.FirstOrDefault(p => p.Code == code);
            if (product == null)
                return NotFound();

            _products.Remove(product);
            return NoContent();
        }
    }
} 