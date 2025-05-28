using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pampazon.Models
{
    public class Dispatch
    {
        [Key]
        public string DispatchNumber { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string CarrierCUIT { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(Order))]
        public string OrderId { get; set; } = string.Empty;
        public Order? Order { get; set; }

        public bool IsFinalized { get; set; }
    }
} 