using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pampazon.Models;

/// <summary>
/// Representa una posición de stock en el almacén
/// </summary>
public class StockPosition
{
    /// <summary>
    /// ID único de la posición
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Código del producto almacenado
    /// </summary>
    [Required]
    [ForeignKey(nameof(Product))]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Producto almacenado
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// CUIT del cliente dueño del stock
    /// </summary>
    [Required]
    [ForeignKey(nameof(Client))]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Cliente dueño del stock
    /// </summary>
    public Client? Client { get; set; }

    /// <summary>
    /// Número del recibo que originó la posición
    /// </summary>
    [Required]
    public string ReceiptNumber { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de producto en la posición
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    /// <summary>
    /// Pasillo donde se encuentra la posición
    /// </summary>
    [Required]
    public string Aisle { get; set; } = string.Empty;

    /// <summary>
    /// Sección dentro del pasillo
    /// </summary>
    [Required]
    public string Section { get; set; } = string.Empty;

    /// <summary>
    /// Estante dentro de la sección
    /// </summary>
    [Required]
    public string Shelf { get; set; } = string.Empty;

    /// <summary>
    /// Nivel dentro del estante
    /// </summary>
    [Required]
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de creación de la posición
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    public string GetPositionCode()
    {
        return $"{Aisle}.{Section}.{Shelf}.{Level}";
    }
}
