namespace Pampazon.Models;

/// <summary>
/// Representa una ubicación en el almacén
/// </summary>
public class StockLocation
{
    /// <summary>
    /// Pasillo del almacén
    /// </summary>
    public string Aisle { get; set; }

    /// <summary>
    /// Sección dentro del pasillo
    /// </summary>
    public string Section { get; set; }

    /// <summary>
    /// Estante dentro de la sección
    /// </summary>
    public string Shelf { get; set; }

    /// <summary>
    /// Nivel dentro del estante
    /// </summary>
    public string Level { get; set; }
}
