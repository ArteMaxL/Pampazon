using Microsoft.EntityFrameworkCore;
using Pampazon.Data;
using Pampazon.Models;
using System.Linq.Expressions;

namespace Pampazon.Services;

public class ClientService(PampazonDbContext context) : IClientService
{
    public async Task<IEnumerable<Client>> GetAllAsync() => await context.Clients
        .Select(c => new Client {
            CUIT = c.CUIT,
            BusinessName = c.BusinessName
        })
        .ToListAsync();

    public async Task<Client?> GetAsync(string cuit) => await context.Clients.FindAsync(cuit);

    public async Task<Client> CreateAsync(Client client)
    {
        if (await context.Clients.AnyAsync(c => c.CUIT == client.CUIT))
            throw new InvalidOperationException("Client with this CUIT already exists");

        context.Clients.Add(client);
        await context.SaveChangesAsync();

        return client;
    }

    public async Task UpdateAsync(string cuit, Client client)
    {
        if (cuit != client.CUIT) throw new ArgumentException("CUIT mismatch");

        var existing = await context.Clients.FindAsync(cuit) ?? throw new KeyNotFoundException("Client not found");
        context.Entry(existing).CurrentValues.SetValues(client);
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string cuit)
    {
        var client = await context.Clients.FindAsync(cuit) ?? throw new KeyNotFoundException("Client not found");
        context.Clients.Remove(client);
        
        await context.SaveChangesAsync();
    }

    public async Task<PagedResult<Client>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, bool desc)
    {
        var query = context.Clients.AsQueryable();
        var orderMappings = new Dictionary<string, string> {
            ["businessname"] = nameof(Client.BusinessName),
            ["cuit"] = nameof(Client.CUIT)
        };

        Expression<Func<Client, bool>>? searchPredicate = null;

        if (!string.IsNullOrWhiteSpace(search))
            searchPredicate = c => c.BusinessName.Contains(search!);

        var paged = await query.ApplyPagedResultAsync(page, pageSize, search, orderBy, desc, searchPredicate, orderMappings);
        
        paged.Items = [.. paged.Items.Select(c => new Client {
            CUIT = c.CUIT,
            BusinessName = c.BusinessName
        })];
        
        return paged;
    }
}
