using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Application.Clients.Interfaces;

public interface IAdminClientService
{
    Task<List<Client>> GetAllClientsAsync();
    Task UpdateClientVerificationStatusAsync(int clientId, ClientVerificationStatus newStatus);
}
