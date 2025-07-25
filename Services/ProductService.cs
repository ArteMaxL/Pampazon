using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using System.Linq.Expressions;

namespace Pampazon.Services;

public class ProductService(PampazonDbContext context) : IProductService
{
    public async Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, bool desc)
    {
        var query = context.Products.AsQueryable();
        var orderMappings = new Dictionary<string, string> {
            ["description"] = nameof(Product.Description),
            ["code"] = nameof(Product.Code)
        };

        Expression<Func<Product, bool>>? searchPredicate = null;

        if (!string.IsNullOrWhiteSpace(search))
            searchPredicate = p => p.Description.Contains(search!);

        var paged = await query.ApplyPagedResultAsync(page, pageSize, search, orderBy, desc, searchPredicate, orderMappings);
        
        paged.Items = [.. paged.Items.Select(p => new Product {
            Code = p.Code,
            Description = p.Description,
            Height = p.Height,
            Width = p.Width,
            Depth = p.Depth
        })];

        return paged;
    }

    public async Task<IEnumerable<Product>> GetAllAsync() => await context.Products
        .Select(p => new Product {
            Code = p.Code,
            Description = p.Description,
            Height = p.Height,
            Width = p.Width,
            Depth = p.Depth
        })
        .ToListAsync();

    public async Task<Product?> GetAsync(string code) => await context.Products.FindAsync(code);

    public async Task<Product> CreateAsync(Product product)
    {
        if (await context.Products.AnyAsync(p => p.Code == product.Code))
            throw new InvalidOperationException("A product with this code already exists");
        
        context.Products.Add(product);
        
        await context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(string code, Product product)
    {
        if (code != product.Code) throw new ArgumentException();
        
        var existingProduct = await context.Products.FindAsync(code) ?? throw new KeyNotFoundException();
        
        context.Entry(existingProduct).CurrentValues.SetValues(product);

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string code)
    {
        var product = await context.Products.FindAsync(code) ?? throw new KeyNotFoundException();
       
        context.Products.Remove(product);

        await context.SaveChangesAsync();
    }
}
