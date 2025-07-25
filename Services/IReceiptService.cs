using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Services;

public interface IReceiptService
{
    Task<PagedResult<Receipt>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, bool desc);
    Task<IEnumerable<Receipt>> GetAllAsync();
    Task<Receipt?> GetAsync(string receiptNumber);
    Task<Receipt> CreateAsync(Receipt receipt);
    Task UpdateStatusAsync(string receiptNumber, ReceiptStatus newStatus);
    Task AssignLocationAsync(string receiptNumber, string productId, StockLocation location);
}
