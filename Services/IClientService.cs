using Pampazon.Models;

namespace Pampazon.Services;

public interface IClientService
{
    Task<PagedResult<Client>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, bool desc);
    Task<IEnumerable<Client>> GetAllAsync();
    Task<Client?> GetAsync(string cuit);
    Task<Client> CreateAsync(Client client);
    Task UpdateAsync(string cuit, Client client);
    Task DeleteAsync(string cuit);
}
