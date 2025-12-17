using SomosRentWi.Application.Clients.Interfaces;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.Enums;
using SomosRentWi.Domain.IRepositories;


namespace SomosRentWi.Application.Clients.Services;

public class AdminClientService : IAdminClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminClientService(IClientRepository clientRepository, IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Client>> GetAllClientsAsync()
    {
        return await _clientRepository.GetAllAsync();
    }

    public async Task UpdateClientVerificationStatusAsync(int clientId, ClientVerificationStatus newStatus)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null)
            throw new Exception("Client not found");

        client.VerificationStatus = newStatus;
        
        await _clientRepository.UpdateAsync(client);
        await _unitOfWork.SaveChangesAsync();
    }
}
