using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Domain.IRepositories;

public interface IClientRepository
{
    Task AddAsync(Client client);
}