using SomosRentWi.Domain.IRepositories;
using SomosRentWi.Infrastructure.Persistence;

namespace SomosRentWi.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly RentWiDbContext _context;

    public UnitOfWork(RentWiDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}