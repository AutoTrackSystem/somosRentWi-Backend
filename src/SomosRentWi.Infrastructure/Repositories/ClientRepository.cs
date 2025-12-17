using Microsoft.EntityFrameworkCore;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.IRepositories;
using SomosRentWi.Infrastructure.Persistence;

namespace SomosRentWi.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly RentWiDbContext _context;

    public ClientRepository(RentWiDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _context.Clients
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Client?> GetByUserIdAsync(int userId)
    {
        return await _context.Clients
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
    }

    public Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        return Task.CompletedTask;
    }

    public async Task<List<Client>> GetAllAsync()
    {
        return await _context.Clients
            .Include(c => c.User)
            .ToListAsync();
    }
}