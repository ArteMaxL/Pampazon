using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pampazon.Enums;

namespace Pampazon.Models
{
    public class Receipt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [ForeignKey(nameof(Client))]
        public string ClientId { get; set; } = string.Empty;
        public Client? Client { get; set; }

        [Required]
        public string CarrierCUIT { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(Order))]
        public string OrderId { get; set; } = string.Empty;
        public Order? Order { get; set; }

        public ReceiptStatus Status { get; set; } = ReceiptStatus.PendingEntry;

        public List<ReceiptItem> Items { get; set; } = new();
    }

    public class ReceiptItem
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
        [ForeignKey(nameof(Receipt))]
        public int ReceiptId { get; set; }
        public Receipt? Receipt { get; set; }
    }
} 