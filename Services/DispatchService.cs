using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using Pampazon.Enums;

namespace Pampazon.Services;

public class DispatchService(PampazonDbContext context) : IDispatchService
{
    public async Task<IEnumerable<Dispatch>> GetAllAsync()
        => await context.Dispatches
            .Select(d => new Dispatch {
                DispatchNumber = d.DispatchNumber,
                OrderNumber = d.OrderNumber,
                Status = d.Status,
                CreatedAt = d.CreatedAt,
                DeliveredAt = d.DeliveredAt,
                CarrierCUIT = d.CarrierCUIT,
                IsFinalized = d.IsFinalized,
                Order = new Order {
                    OrderNumber = d.Order.OrderNumber,
                    ClientId = d.Order.ClientId,
                    Status = d.Order.Status,
                    Date = d.Order.Date,
                    Client = d.Order.Client,
                    Items = d.Order.Items.Select(i => new OrderItem {
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
                }
            })
            .ToListAsync();

    public async Task<Dispatch?> GetAsync(string dispatchNumber)
        => await context.Dispatches
            .Include(d => d.Order)
                .ThenInclude(o => o.Client)
            .Include(d => d.Order)
                .ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(d => d.DispatchNumber == dispatchNumber);

    public async Task<Dispatch> CreateForOrderAsync(string orderNumber)
    {
        var order = await context.Orders.FindAsync(orderNumber) ?? throw new KeyNotFoundException("Order not found");
        if (order.Status != OrderStatus.Prepared)
            throw new InvalidOperationException("Order is not ready for dispatch");

        var lastDispatch = await context.Dispatches.OrderByDescending(d => d.DispatchNumber).FirstOrDefaultAsync();
        int counter = 1;
        if (lastDispatch != null && int.TryParse(lastDispatch.DispatchNumber[4..], out int lastNumber))
            counter = lastNumber + 1;

        var dispatch = new Dispatch
        {
            DispatchNumber = $"DISP{counter:D6}",
            OrderNumber = orderNumber,
            Status = DispatchStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        order.Status = OrderStatus.Dispatched;
        context.Dispatches.Add(dispatch);
        await context.SaveChangesAsync();
        return dispatch;
    }

    public async Task UpdateStatusAsync(string dispatchNumber, DispatchStatus newStatus)
    {
        var dispatch = await context.Dispatches.FindAsync(dispatchNumber) ?? throw new KeyNotFoundException("Dispatch not found");
        if (dispatch.Status == DispatchStatus.Delivered)
            throw new InvalidOperationException("Cannot update status of delivered dispatches");
        if (newStatus == DispatchStatus.Delivered && dispatch.Status != DispatchStatus.InTransit)
            throw new InvalidOperationException("Can only mark in-transit dispatches as delivered");
        dispatch.Status = newStatus;
        if (newStatus == DispatchStatus.Delivered)
            dispatch.DeliveredAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }
}
