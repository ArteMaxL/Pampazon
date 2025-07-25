using Pampazon.Models;

namespace Pampazon.Services;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public interface IProductService
{
    Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, bool desc);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetAsync(string code);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(string code, Product product);
    Task DeleteAsync(string code);
}
