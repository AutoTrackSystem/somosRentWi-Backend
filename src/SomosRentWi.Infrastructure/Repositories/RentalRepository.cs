using Microsoft.EntityFrameworkCore;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.IRepositories;
using SomosRentWi.Infrastructure.Persistence;

namespace SomosRentWi.Infrastructure.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly RentWiDbContext _context;

    public RentalRepository(RentWiDbContext context)
    {
        _context = context;
    }

    public async Task<Rental?> GetByIdAsync(int id)
    {
        return await _context.Rentals
            .Include(r => r.Client)
                .ThenInclude(c => c.User)
            .Include(r => r.Company)
                .ThenInclude(c => c.User)
            .Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Rental>> GetByClientIdAsync(int clientId)
    {
        return await _context.Rentals
            .Include(r => r.Client)
                .ThenInclude(c => c.User)
            .Include(r => r.Company)
                .ThenInclude(c => c.User)
            .Include(r => r.Car)
            .Where(r => r.ClientId == clientId)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }

    public async Task<List<Rental>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.Rentals
            .Include(r => r.Client)
                .ThenInclude(c => c.User)
            .Include(r => r.Company)
                .ThenInclude(c => c.User)
            .Include(r => r.Car)
            .Where(r => r.CompanyId == companyId)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }

    public async Task<List<Rental>> GetByCarIdAsync(int carId)
    {
        return await _context.Rentals
            .Include(r => r.Client)
                .ThenInclude(c => c.User)
            .Include(r => r.Company)
                .ThenInclude(c => c.User)
            .Include(r => r.Car)
            .Where(r => r.CarId == carId)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }

    public async Task<List<Rental>> GetAllAsync()
    {
        return await _context.Rentals
            .Include(r => r.Client)
                .ThenInclude(c => c.User)
            .Include(r => r.Company)
                .ThenInclude(c => c.User)
            .Include(r => r.Car)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }

    public async Task AddAsync(Rental rental)
    {
        await _context.Rentals.AddAsync(rental);
    }

    public Task UpdateAsync(Rental rental)
    {
        _context.Rentals.Update(rental);
        return Task.CompletedTask;
    }
}
