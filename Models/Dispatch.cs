using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pampazon.Enums;

namespace Pampazon.Models
{
    /// <summary>
    /// Representa un despacho de mercadería
    /// </summary>
    public class Dispatch
    {
        /// <summary>
        /// Número único del despacho (formato: DISPxxxxxx)
        /// </summary>
        [Key]
        public string DispatchNumber { get; set; } = string.Empty;

        /// <summary>
        /// Número de la orden asociada
        /// </summary>
        [Required]
        [ForeignKey(nameof(Order))]
        public string OrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// Orden asociada al despacho
        /// </summary>
        public Order? Order { get; set; }

        /// <summary>
        /// Estado actual del despacho
        /// </summary>
        [Required]
        public DispatchStatus Status { get; set; }

        /// <summary>
        /// Fecha de creación del despacho
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de entrega del despacho
        /// </summary>
        public DateTime? DeliveredAt { get; set; }

        [Required]
        public string CarrierCUIT { get; set; } = string.Empty;

        public bool IsFinalized { get; set; }
    }
} 