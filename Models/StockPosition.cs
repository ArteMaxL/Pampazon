using System.ComponentModel.DataAnnotations;

namespace Pampazon.Models
{
    public class StockPosition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression("[A-Z]")]
        public char Aisle { get; set; }  // Pasillo

        [Required]
        [Range(1, int.MaxValue)]
        public int Section { get; set; }  // Sección

        [Required]
        [Range(1, int.MaxValue)]
        public int Shelf { get; set; }    // Estantería

        [Required]
        [Range(1, int.MaxValue)]
        public int Level { get; set; }    // Nivel

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public string ProductCode { get; set; } = string.Empty;
        public Product? Product { get; set; }

        public string ClientId { get; set; } = string.Empty;  // CUIT
        public Client? Client { get; set; }

        public string GetPositionCode()
        {
            return $"{Aisle}.{Section}.{Shelf}.{Level}";
        }
    }
} 