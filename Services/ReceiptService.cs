using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using Pampazon.Enums;
using System.Linq.Expressions;

namespace Pampazon.Services;

public class ReceiptService(PampazonDbContext context)
    : IReceiptService
{
    public async Task<IEnumerable<Receipt>> GetAllAsync()
        => await context.Receipts
            .Select(r => new Receipt {
                ReceiptNumber = r.ReceiptNumber,
                Date = r.Date,
                Status = r.Status,
                ClientId = r.ClientId,
                Client = r.Client,
                Items = r.Items.Select(i => new ReceiptItem {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    ReceiptNumber = i.ReceiptNumber,
                    Product = new Product {
                        Code = i.Product.Code,
                        Description = i.Product.Description,
                        Height = i.Product.Height,
                        Width = i.Product.Width,
                        Depth = i.Product.Depth
                    }
                }).ToList()
            })
            .ToListAsync();

    public async Task<Receipt?> GetAsync(string receiptNumber)
        => await context.Receipts
            .Include(r => r.Client)
            .Include(r => r.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(r => r.ReceiptNumber == receiptNumber);

    public async Task<Receipt> CreateAsync(Receipt receipt)
    {
        await using var tx = await context.Database.BeginTransactionAsync();

        // Validate client
        if (!await context.Clients.AnyAsync(c => c.CUIT == receipt.ClientId))
            throw new InvalidOperationException("Client not found");

        // Validate products
        foreach (var item in receipt.Items)
        {
            if (!await context.Products.AnyAsync(p => p.Code == item.ProductId))
                throw new InvalidOperationException($"Product {item.ProductId} not found");
        }

        // Generate sequential number
        var last = await context.Receipts.OrderByDescending(r => r.ReceiptNumber).FirstOrDefaultAsync();
        int counter = 1;

        if (last != null && int.TryParse(last.ReceiptNumber[3..], out int lastNum))
            counter = lastNum + 1;

        receipt.ReceiptNumber = $"RCP{counter:D6}";
        receipt.Date = DateTime.UtcNow;
        receipt.Status = ReceiptStatus.Pending;

        context.Receipts.Add(receipt);

        if (receipt.Items?.Any() == true)
        {
            foreach (var i in receipt.Items)
                i.ReceiptNumber = receipt.ReceiptNumber;

            context.ReceiptItems.AddRange(receipt.Items);
        }

        await context.SaveChangesAsync();
        await tx.CommitAsync();

        return receipt;
    }

    public async Task UpdateStatusAsync(string receiptNumber, ReceiptStatus newStatus)
    {
        var receipt = await context.Receipts.Include(r => r.Items).FirstOrDefaultAsync(r => r.ReceiptNumber == receiptNumber)
            ?? throw new KeyNotFoundException("Receipt not found");

        if (receipt.Status == ReceiptStatus.Completed)
            throw new InvalidOperationException("Cannot update completed receipt");

        if (newStatus == ReceiptStatus.Completed && receipt.Status != ReceiptStatus.InProgress)
            throw new InvalidOperationException("Can only complete receipts in progress");

        receipt.Status = newStatus;
        if (newStatus == ReceiptStatus.Completed)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            receipt.CompletedAt = DateTime.UtcNow;
            var positions = receipt.Items.Select(i => new StockPosition
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                ClientId = receipt.ClientId,
                ReceiptNumber = receipt.ReceiptNumber,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            context.StockPositions.AddRange(positions);
            await context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        else
        {
            await context.SaveChangesAsync();
        }
    }

    public async Task AssignLocationAsync(string receiptNumber, string productId, StockLocation location)
    {
        var receipt = await context.Receipts.Include(r => r.Items).FirstOrDefaultAsync(r => r.ReceiptNumber == receiptNumber)
            ?? throw new KeyNotFoundException("Receipt not found");

        if (receipt.Status != ReceiptStatus.InProgress)
            throw new InvalidOperationException("Receipt not in progress");

        var item = receipt.Items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new KeyNotFoundException("Product not found in receipt");

        bool occupied = await context.StockPositions.AnyAsync(p => p.Aisle == location.Aisle && p.Section == location.Section && p.Shelf == location.Shelf && p.Level == location.Level);
        if (occupied) throw new InvalidOperationException("Location already in use");

        var pos = await context.StockPositions.FirstOrDefaultAsync(p => p.ReceiptNumber == receiptNumber && p.ProductId == productId);
        if (pos == null)
        {
            pos = new StockPosition
            {
                ProductId = productId,
                Quantity = item.Quantity,
                ClientId = receipt.ClientId,
                ReceiptNumber = receiptNumber,
                CreatedAt = DateTime.UtcNow
            };
            context.StockPositions.Add(pos);
        }

        pos.Aisle = location.Aisle;
        pos.Section = location.Section;
        pos.Shelf = location.Shelf;
        pos.Level = location.Level;

        await context.SaveChangesAsync();
    }

    public async Task<PagedResult<Receipt>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, bool desc)
    {
        var query = context.Receipts.AsQueryable();
        var orderMappings = new Dictionary<string, string> {
            ["date"] = nameof(Receipt.Date),
            ["receiptnumber"] = nameof(Receipt.ReceiptNumber),
            ["status"] = nameof(Receipt.Status)
        };

        Expression<Func<Receipt, bool>>? searchPredicate = null;

        if (!string.IsNullOrWhiteSpace(search))
            searchPredicate = r => r.ReceiptNumber.Contains(search!) || r.ClientId.Contains(search!);
        
        var paged = await query.ApplyPagedResultAsync(page, pageSize, search, orderBy, desc, searchPredicate, orderMappings);
        
        paged.Items = [.. paged.Items.Select(r => new Receipt {
            ReceiptNumber = r.ReceiptNumber,
            Date = r.Date,
            Status = r.Status,
            ClientId = r.ClientId,
            Client = r.Client,
            Items = [.. r.Items.Select(i => new ReceiptItem {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                ReceiptNumber = i.ReceiptNumber,
                Product = new Product {
                    Code = i.Product.Code,
                    Description = i.Product.Description,
                    Height = i.Product.Height,
                    Width = i.Product.Width,
                    Depth = i.Product.Depth
                }
            })]
        })];

        return paged;
    }
}
