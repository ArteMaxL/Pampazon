using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;

namespace Pampazon.Controllers
{
    /// <summary>
    /// Controlador para la gestión de productos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly PampazonDbContext _context;

        public ProductsController(PampazonDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los productos registrados
        /// </summary>
        /// <returns>Lista de productos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            return Ok(await _context.Products.ToListAsync());
        }

        /// <summary>
        /// Obtiene un producto específico por su código
        /// </summary>
        /// <param name="code">Código del producto</param>
        /// <returns>Producto solicitado</returns>
        [HttpGet("{code}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> Get(string code)
        {
            var product = await _context.Products.FindAsync(code);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        /// <summary>
        /// Registra un nuevo producto
        /// </summary>
        /// <param name="product">Datos del producto a registrar</param>
        /// <returns>Producto creado</returns>
        /// <response code="201">Producto creado exitosamente</response>
        /// <response code="409">Ya existe un producto con el código especificado</response>
        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            if (await _context.Products.AnyAsync(p => p.Code == product.Code))
                return Conflict("A product with this code already exists");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { code = product.Code }, product);
        }

        /// <summary>
        /// Actualiza los datos de un producto existente
        /// </summary>
        /// <param name="code">Código del producto a actualizar</param>
        /// <param name="product">Nuevos datos del producto</param>
        /// <returns>No content si la actualización es exitosa</returns>
        /// <response code="204">Producto actualizado exitosamente</response>
        /// <response code="400">El código en la URL no coincide con el del producto</response>
        /// <response code="404">No se encontró el producto especificado</response>
        [HttpPut("{code}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Elimina un producto
        /// </summary>
        /// <param name="code">Código del producto a eliminar</param>
        /// <returns>No content si la eliminación es exitosa</returns>
        /// <response code="204">Producto eliminado exitosamente</response>
        /// <response code="404">No se encontró el producto especificado</response>
        [HttpDelete("{code}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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