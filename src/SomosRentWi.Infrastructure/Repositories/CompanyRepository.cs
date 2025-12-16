using Microsoft.EntityFrameworkCore;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.IRepositories;
using SomosRentWi.Infrastructure.Persistence;

namespace SomosRentWi.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly RentWiDbContext _context;

    public CompanyRepository(RentWiDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByNitAsync(string nit)
    {
        return await _context.Companies.AnyAsync(c => c.NitNumber == nit);
    }

    public async Task AddAsync(Company company)
    {
        await _context.Companies.AddAsync(company);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}