using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Services;

public interface IDispatchService
{
    Task<IEnumerable<Dispatch>> GetAllAsync();
    Task<Dispatch?> GetAsync(string dispatchNumber);
    Task<Dispatch> CreateForOrderAsync(string orderNumber);
    Task UpdateStatusAsync(string dispatchNumber, DispatchStatus newStatus);
}
