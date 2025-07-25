using Pampazon.Models;

namespace Pampazon.Services;

public interface IStockService
{
    Task<PagedResult<StockPosition>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, bool desc);
    Task<IEnumerable<StockPosition>> GetAllAsync();
    Task<IEnumerable<StockPosition>> GetByProductAsync(string productId);
    Task<StockPosition> CreateAsync(StockPosition position);
    Task UpdateQuantityAsync(int id, int quantity);
    Task DeleteAsync(int id);
}
