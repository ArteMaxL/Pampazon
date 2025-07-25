using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using System.Linq.Expressions;

namespace Pampazon.Services;

public class StockService(PampazonDbContext context) : IStockService
{
    public async Task<IEnumerable<StockPosition>> GetAllAsync()
        => await context.StockPositions
            .Select(p => new StockPosition {
                Id = p.Id,
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                ClientId = p.ClientId,
                ReceiptNumber = p.ReceiptNumber,
                CreatedAt = p.CreatedAt,
                Aisle = p.Aisle,
                Section = p.Section,
                Shelf = p.Shelf,
                Level = p.Level,
                Product = new Product {
                    Code = p.Product.Code,
                    Description = p.Product.Description,
                    Height = p.Product.Height,
                    Width = p.Product.Width,
                    Depth = p.Product.Depth
                },
                Client = new Client {
                    CUIT = p.Client.CUIT,
                    BusinessName = p.Client.BusinessName
                }
            })
            .ToListAsync();

    public async Task<IEnumerable<StockPosition>> GetByProductAsync(string productId)
        => await context.StockPositions
            .Where(p => p.ProductId == productId)
            .Select(p => new StockPosition {
                Id = p.Id,
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                ClientId = p.ClientId,
                ReceiptNumber = p.ReceiptNumber,
                CreatedAt = p.CreatedAt,
                Aisle = p.Aisle,
                Section = p.Section,
                Shelf = p.Shelf,
                Level = p.Level,
                Product = new Product {
                    Code = p.Product.Code,
                    Description = p.Product.Description,
                    Height = p.Product.Height,
                    Width = p.Product.Width,
                    Depth = p.Product.Depth
                },
                Client = new Client {
                    CUIT = p.Client.CUIT,
                    BusinessName = p.Client.BusinessName
                }
            })
            .ToListAsync();

    public async Task<StockPosition> CreateAsync(StockPosition position)
    {
        if (!await context.Products.AnyAsync(p => p.Code == position.ProductId))
            throw new InvalidOperationException($"El producto con código {position.ProductId} no existe");
        
        if (!await context.Clients.AnyAsync(c => c.CUIT == position.ClientId))
            throw new InvalidOperationException($"El cliente con CUIT {position.ClientId} no existe");
        
        if (!await context.Receipts.AnyAsync(r => r.ReceiptNumber == position.ReceiptNumber))
            throw new InvalidOperationException($"El recibo {position.ReceiptNumber} no existe");
        
        var exists = await context.StockPositions.AnyAsync(p => p.Aisle == position.Aisle && p.Section == position.Section && p.Shelf == position.Shelf && p.Level == position.Level);
       
        if (exists)
            throw new ArgumentException("Esta posición ya está en uso");
        
        position.CreatedAt = DateTime.UtcNow;
        context.StockPositions.Add(position);
        
        await context.SaveChangesAsync();
        return position;
    }

    public async Task UpdateQuantityAsync(int id, int quantity)
    {
        var position = await context.StockPositions.FindAsync(id) ?? throw new KeyNotFoundException();
        
        position.Quantity = quantity;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var position = await context.StockPositions.FindAsync(id) ?? throw new KeyNotFoundException();
        
        context.StockPositions.Remove(position);

        await context.SaveChangesAsync();
    }

    public async Task<PagedResult<StockPosition>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, bool desc)
    {
        var query = context.StockPositions.AsQueryable();
        var orderMappings = new Dictionary<string, string> {
            ["productid"] = nameof(StockPosition.ProductId),
            ["clientid"] = nameof(StockPosition.ClientId),
            ["quantity"] = nameof(StockPosition.Quantity)
        };

        Expression<Func<StockPosition, bool>>? searchPredicate = null;
        
        if (!string.IsNullOrWhiteSpace(search))
            searchPredicate = p => p.ProductId.Contains(search!) || p.ClientId.Contains(search!);
        
        var paged = await query.ApplyPagedResultAsync(page, pageSize, search, orderBy, desc, searchPredicate, orderMappings);
        
        paged.Items = [.. paged.Items.Select(p => new StockPosition {
            Id = p.Id,
            ProductId = p.ProductId,
            Quantity = p.Quantity,
            ClientId = p.ClientId,
            ReceiptNumber = p.ReceiptNumber,
            CreatedAt = p.CreatedAt,
            Aisle = p.Aisle,
            Section = p.Section,
            Shelf = p.Shelf,
            Level = p.Level,
            Product = p.Product == null ? null : new Product {
                Code = p.Product.Code,
                Description = p.Product.Description,
                Height = p.Product.Height,
                Width = p.Product.Width,
                Depth = p.Product.Depth
            },
            Client = p.Client == null ? null : new Client {
                CUIT = p.Client.CUIT,
                BusinessName = p.Client.BusinessName
            }
        })];

        return paged;
    }
}
