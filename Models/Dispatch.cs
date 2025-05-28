using System.ComponentModel.DataAnnotations;

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

        public bool IsFinalized { get; set; }

        public List<Order> Orders { get; set; } = new();
    }
} 