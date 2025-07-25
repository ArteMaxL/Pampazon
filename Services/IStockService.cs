using Pampazon.Models;

namespace Pampazon.Services;

public interface IStockService
{
    Task<IEnumerable<StockPosition>> GetAllAsync();
    Task<IEnumerable<StockPosition>> GetByProductAsync(string productId);
    Task<StockPosition> CreateAsync(StockPosition position);
    Task UpdateQuantityAsync(int id, int quantity);
    Task DeleteAsync(int id);
}
