namespace Pampazon.Enums;

/// <summary>
/// Estados posibles de un recibo de mercadería
/// </summary>
public enum ReceiptStatus
{
    /// <summary>
    /// Recibo pendiente de iniciar
    /// </summary>
    Pending,

    /// <summary>
    /// Recibo en proceso de ubicación
    /// </summary>
    InProgress,

    /// <summary>
    /// Recibo completado
    /// </summary>
    Completed
}
