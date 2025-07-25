using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Services;

public interface IDispatchService
{
    Task<PagedResult<Dispatch>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, bool desc);
    Task<IEnumerable<Dispatch>> GetAllAsync();
    Task<Dispatch?> GetAsync(string dispatchNumber);
    Task<Dispatch> CreateForOrderAsync(string orderNumber);
    Task UpdateStatusAsync(string dispatchNumber, DispatchStatus newStatus);
}
