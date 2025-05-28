using System.ComponentModel.DataAnnotations;
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
        public string ClientCUIT { get; set; } = string.Empty;
        public Client? Client { get; set; }

        [Required]
        public string CarrierCUIT { get; set; } = string.Empty;

        public ReceiptStatus Status { get; set; } = ReceiptStatus.PendingEntry;

        public List<ReceiptItem> Items { get; set; } = new();
    }

    public class ReceiptItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProductCode { get; set; } = string.Empty;
        public Product? Product { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public int ReceiptId { get; set; }
        public Receipt? Receipt { get; set; }
    }
} 