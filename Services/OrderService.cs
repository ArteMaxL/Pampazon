using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Services;

public class OrderService(PampazonDbContext context) : IOrderService
{
    public async Task<IEnumerable<Order>> GetAllAsync()
        => await context.Orders
            .Select(o => new Order {
                OrderNumber = o.OrderNumber,
                Date = o.Date,
                Status = o.Status,
                ClientId = o.ClientId,
                Client = o.Client,
                Items = o.Items.Select(i => new OrderItem {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    OrderNumber = i.OrderNumber,
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

    public async Task<Order?> GetAsync(string orderNumber)
        => await context.Orders
            .Include(o => o.Client)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

    public async Task<Order> CreateAsync(Order order)
    {
        await using var tx = await context.Database.BeginTransactionAsync();

        // generate sequential order number
        var last = await context.Orders.OrderByDescending(o => o.OrderNumber).FirstOrDefaultAsync();
        int counter = 1;
        if (last != null && int.TryParse(last.OrderNumber[3..], out int lastNum)) counter = lastNum + 1;
        order.OrderNumber = $"ORD{counter:D6}";
        order.Date = DateTime.UtcNow;
        order.Status = OrderStatus.Pending;

        context.Orders.Add(order);
        if (order.Items?.Any() == true)
        {
            foreach (var it in order.Items)
                it.OrderNumber = order.OrderNumber;
            context.OrderItems.AddRange(order.Items);
        }
        await context.SaveChangesAsync();
        await tx.CommitAsync();
        return order;
    }

    public async Task UpdateStatusAsync(string orderNumber, OrderStatus newStatus)
    {
        var order = await context.Orders.FindAsync(orderNumber) ?? throw new KeyNotFoundException();

        if (order.Status != OrderStatus.Pending && newStatus == OrderStatus.Prepared)
            throw new InvalidOperationException("Can only prepare pending orders");
        if (order.Status != OrderStatus.Prepared && newStatus == OrderStatus.Dispatched)
            throw new InvalidOperationException("Can only dispatch prepared orders");

        order.Status = newStatus;
        await context.SaveChangesAsync();
    }

    public async Task AssignPositionsAsync(string orderNumber, List<int> positionIds)
    {
        var order = await context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderNumber == orderNumber)
            ?? throw new KeyNotFoundException("Order not found");

        if (order.Status != OrderStatus.Pending)
            throw new InvalidOperationException("Can only assign positions to pending orders");

        var positions = await context.StockPositions.Include(p => p.Product).Where(p => positionIds.Contains(p.Id)).ToListAsync();
        if (positions.Count != positionIds.Count)
            throw new InvalidOperationException("Some positions not found");

        foreach (var item in order.Items)
        {
            var available = positions.Where(p => p.ProductId == item.ProductId).Sum(p => p.Quantity);
            if (available < item.Quantity)
                throw new InvalidOperationException($"Not enough stock for product {item.ProductId}");
        }

        foreach (var item in order.Items)
        {
            var remaining = item.Quantity;
            foreach (var pos in positions.Where(p => p.ProductId == item.ProductId).OrderBy(p => p.Quantity))
            {
                if (remaining <= 0) break;
                var take = Math.Min(remaining, pos.Quantity);
                pos.Quantity -= take;
                remaining -= take;
            }
        }

        order.Status = OrderStatus.Prepared;
        await context.SaveChangesAsync();
    }
}
