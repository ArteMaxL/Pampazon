using System.ComponentModel.DataAnnotations;

namespace Pampazon.Models;

public class Product
{
    [Key]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public decimal Height { get; set; }  // en centimetros
    
    [Required]
    public decimal Width { get; set; }   // en centimetros
    
    [Required]
    public decimal Depth { get; set; }   // en centimetros

    public List<StockPosition> StockPositions { get; set; } = new();
}
