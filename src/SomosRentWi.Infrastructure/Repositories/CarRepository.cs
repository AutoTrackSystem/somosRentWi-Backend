using Microsoft.EntityFrameworkCore;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.IRepositories;
using SomosRentWi.Infrastructure.Persistence;

namespace SomosRentWi.Infrastructure.Repositories;

public class CarRepository : ICarRepository
{
    private readonly RentWiDbContext _context;

    public CarRepository(RentWiDbContext context)
    {
        _context = context;
    }

    public async Task<Car?> GetByIdAsync(int id)
    {
        return await _context.Cars
            .Include(c => c.Company)
                .ThenInclude(c => c.Wallet)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Car>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.Cars
            .Include(c => c.Company)
            .Where(c => c.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<List<Car>> GetAllAsync()
    {
        return await _context.Cars
            .Include(c => c.Company)
            .ToListAsync();
    }

    public async Task AddAsync(Car car)
    {
        await _context.Cars.AddAsync(car);
    }

    public Task UpdateAsync(Car car)
    {
        _context.Cars.Update(car);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Car car)
    {
        _context.Cars.Remove(car);
        return Task.CompletedTask;
    }
}
