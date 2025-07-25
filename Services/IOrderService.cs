using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Services;

public interface IOrderService
{
    Task<PagedResult<Order>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, bool desc);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetAsync(string orderNumber);
    Task<Order> CreateAsync(Order order);
    Task UpdateStatusAsync(string orderNumber, OrderStatus newStatus);
    Task AssignPositionsAsync(string orderNumber, List<int> positionIds);
}
