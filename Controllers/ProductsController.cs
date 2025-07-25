using Microsoft.AspNetCore.Mvc;
using Pampazon.Models;
using Pampazon.Services;

namespace Pampazon.Controllers;

/// <summary>
/// Controlador para la gestión de productos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    public ProductsController(IProductService service) => _service = service;

    /// <summary>
    /// Obtiene todos los productos registrados
    /// </summary>
    /// <returns>Lista de productos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
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
        var product = await _service.GetAsync(code);
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
        try
        {
            var created = await _service.CreateAsync(product);
            return CreatedAtAction(nameof(Get), new { code = created.Code }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Conflicto", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
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
        try
        {
            await _service.UpdateAsync(code, product);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return Problem(title: "Datos inválidos", detail: "El código en la URL no coincide con el del producto", statusCode: StatusCodes.Status400BadRequest);
        }
        catch (KeyNotFoundException)
        {
            return Problem(title: "No encontrado", detail: "No se encontró el producto especificado", statusCode: StatusCodes.Status404NotFound);
        }
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
        try
        {
            await _service.DeleteAsync(code);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Problem(title: "No encontrado", detail: "No se encontró el producto especificado", statusCode: StatusCodes.Status404NotFound);
        }
    }
}
