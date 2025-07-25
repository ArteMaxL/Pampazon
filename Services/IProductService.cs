using Pampazon.Models;

namespace Pampazon.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetAsync(string code);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(string code, Product product);
    Task DeleteAsync(string code);
}
