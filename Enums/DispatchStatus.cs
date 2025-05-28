namespace Pampazon.Enums
{
    /// <summary>
    /// Estados posibles de un despacho
    /// </summary>
    public enum DispatchStatus
    {
        /// <summary>
        /// Despacho pendiente de iniciar
        /// </summary>
        Pending,

        /// <summary>
        /// Despacho en tránsito
        /// </summary>
        InTransit,

        /// <summary>
        /// Despacho entregado
        /// </summary>
        Delivered
    }
} 