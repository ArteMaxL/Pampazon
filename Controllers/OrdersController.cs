using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using Pampazon.Models;
using Pampazon.Enums;
using System.ComponentModel.DataAnnotations;

using Pampazon.Services;

namespace Pampazon.Controllers;

/// <summary>
/// Controlador para la gestión de órdenes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    public OrdersController(IOrderService service) => _service = service;

    /// <summary>
    /// Obtiene todas las órdenes con sus items y productos asociados
    /// </summary>
    /// <returns>Lista de órdenes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll()
    {
        var orders = await _service.GetAllAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Obtiene una orden específica por su número
    /// </summary>
    /// <param name="orderNumber">Número de orden (formato: ORDxxxxxx)</param>
    /// <returns>Orden solicitada</returns>
    [HttpGet("{orderNumber}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order>> Get(string orderNumber)
    {
        var order = await _service.GetAsync(orderNumber);

        if (order == null)
            return Problem(title: "No encontrado", detail: "No se encontró la orden especificada", statusCode: StatusCodes.Status404NotFound);
        
        return Ok(order);
    }

    /// <summary>
    /// Crea una nueva orden
    /// </summary>
    /// <param name="order">Datos de la orden</param>
    /// <returns>Orden creada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Order>> Create(Order order)
    {
        try
        {
            var created = await _service.CreateAsync(order);
            return CreatedAtAction(nameof(Get), new { orderNumber = created.OrderNumber }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Datos inválidos", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Actualiza el estado de una orden
    /// </summary>
    /// <param name="orderNumber">Número de orden</param>
    /// <param name="newStatus">Nuevo estado</param>
    /// <returns>No content si la actualización es exitosa</returns>
    [HttpPost("{orderNumber}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(string orderNumber, [FromBody][Required] OrderStatus newStatus)
    {
        try
        {
            await _service.UpdateStatusAsync(orderNumber, newStatus);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Problem(title: "No encontrado", detail: "No se encontró la orden especificada", statusCode: StatusCodes.Status404NotFound);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Datos inválidos", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Asigna posiciones de stock a una orden y la marca como preparada
    /// </summary>
    /// <param name="orderNumber">Número de orden</param>
    /// <param name="positionIds">Lista de IDs de posiciones de stock</param>
    /// <returns>No content si la asignación es exitosa</returns>
    [HttpPost("{orderNumber}/positions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignPositions(string orderNumber, [FromBody][Required] List<int> positionIds)
    {
        try
        {
            await _service.AssignPositionsAsync(orderNumber, positionIds);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Problem(title: "No encontrado", detail: "No se encontró la orden especificada", statusCode: StatusCodes.Status404NotFound);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Datos inválidos", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Obtiene órdenes paginadas, filtradas y ordenadas
    /// </summary>
    /// <param name="page">Página (por defecto 1)</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10)</param>
    /// <param name="search">Filtro por número de orden o cliente</param>
    /// <param name="orderBy">Campo de ordenamiento (orderNumber, date, status)</param>
    /// <param name="desc">Orden descendente</param>
    /// <returns>Página de órdenes</returns>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<Order>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<Order>>> GetPaged(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        string? orderBy = null,
        bool desc = false)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search, orderBy, desc);
        return Ok(result);
    }
}
