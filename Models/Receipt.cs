using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pampazon.Enums;

namespace Pampazon.Models;

/// <summary>
/// Representa un recibo de mercadería
/// </summary>
public class Receipt
{
    /// <summary>
    /// Número único del recibo (formato: RCPxxxxxx)
    /// </summary>
    [Key]
    public string ReceiptNumber { get; set; } = string.Empty;

    /// <summary>
    /// CUIT del cliente que envía la mercadería
    /// </summary>
    [Required]
    [ForeignKey(nameof(Client))]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Cliente que envía la mercadería
    /// </summary>
    public Client? Client { get; set; }

    /// <summary>
    /// Fecha de creación del recibo
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// Estado actual del recibo
    /// </summary>
    [Required]
    public ReceiptStatus Status { get; set; }

    /// <summary>
    /// Fecha de completado del recibo
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Items incluidos en el recibo
    /// </summary>
    public List<ReceiptItem> Items { get; set; } = new();
}

/// <summary>
/// Representa un item en un recibo de mercadería
/// </summary>
public class ReceiptItem
{
    /// <summary>
    /// ID del item
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Número del recibo al que pertenece
    /// </summary>
    [Required]
    [ForeignKey(nameof(Receipt))]
    public string ReceiptNumber { get; set; } = string.Empty;

    /// <summary>
    /// Recibo al que pertenece
    /// </summary>
    public Receipt? Receipt { get; set; }

    /// <summary>
    /// Código del producto
    /// </summary>
    [Required]
    [ForeignKey(nameof(Product))]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Producto recibido
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// Cantidad recibida
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
