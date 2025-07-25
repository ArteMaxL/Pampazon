using Pampazon.Models;

namespace Pampazon.Services;

public interface IClientService
{
    Task<IEnumerable<Client>> GetAllAsync();
    Task<Client?> GetAsync(string cuit);
    Task<Client> CreateAsync(Client client);
    Task UpdateAsync(string cuit, Client client);
    Task DeleteAsync(string cuit);
}
