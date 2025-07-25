using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Services;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetAsync(string orderNumber);
    Task<Order> CreateAsync(Order order);
    Task UpdateStatusAsync(string orderNumber, OrderStatus newStatus);
    Task AssignPositionsAsync(string orderNumber, List<int> positionIds);
}
