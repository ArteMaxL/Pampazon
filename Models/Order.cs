using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pampazon.Enums;

namespace Pampazon.Models;

public class Order
{
    [Key]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [ForeignKey(nameof(Client))]
    public string ClientId { get; set; } = string.Empty;
    public Client? Client { get; set; }

    [Required]
    public string RecipientName { get; set; } = string.Empty;

    [Required]
    public string RecipientAddress { get; set; } = string.Empty;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public string? DispatchNumber { get; set; }
    public Dispatch? Dispatch { get; set; }

    public Receipt? Receipt { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(Product))]
    public string ProductId { get; set; } = string.Empty;
    public Product? Product { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [ForeignKey(nameof(Order))]
    public string OrderNumber { get; set; } = string.Empty;
    public Order? Order { get; set; }
}
