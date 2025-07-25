using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Services;

public interface IReceiptService
{
    Task<IEnumerable<Receipt>> GetAllAsync();
    Task<Receipt?> GetAsync(string receiptNumber);
    Task<Receipt> CreateAsync(Receipt receipt);
    Task UpdateStatusAsync(string receiptNumber, ReceiptStatus newStatus);
    Task AssignLocationAsync(string receiptNumber, string productId, StockLocation location);
}
