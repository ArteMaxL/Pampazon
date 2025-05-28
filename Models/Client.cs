using System.ComponentModel.DataAnnotations;

namespace Pampazon.Models;

public class Client
{
    [Key]
    public string CUIT { get; set; } = string.Empty;

    [Required]
    public string BusinessName { get; set; } = string.Empty;  // Razón Social

    public List<StockPosition> RentedPositions { get; set; } = new();
}
