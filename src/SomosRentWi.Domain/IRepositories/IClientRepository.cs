using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Domain.IRepositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id);
    Task<Client?> GetByUserIdAsync(int userId);
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
    Task<List<Client>> GetAllAsync();
}