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

    public async Task<Company?> GetByIdAsync(int id)
    {
        return await _context.Companies
            .Include(c => c.User)
            .Include(c => c.Wallet)
            .Include(c => c.Cars)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Company?> GetByUserIdAsync(int userId)
    {
        return await _context.Companies
            .Include(c => c.User)
            .Include(c => c.Wallet)
            .Include(c => c.Cars)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<List<Company>> GetAllAsync()
    {
        return await _context.Companies
            .Include(c => c.User)
            .Include(c => c.Wallet)
            .ToListAsync();
    }

    public async Task<CompanyWallet?> GetWalletByUserIdAsync(int userId)
    {
        return await _context.Companies
            .Where(c => c.UserId == userId)
            .Select(c => c.Wallet)
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsByNitAsync(string nit)
    {
        return await _context.Companies.AnyAsync(c => c.NitNumber == nit);
    }

    public async Task<Company?> GetByNitAsync(string nit)
    {
        return await _context.Companies
            .Include(c => c.User)
            .Include(c => c.Wallet)
            .FirstOrDefaultAsync(c => c.NitNumber == nit);
    }

    public async Task AddAsync(Company company)
    {
        await _context.Companies.AddAsync(company);
    }

    public Task UpdateAsync(Company company)
    {
        _context.Companies.Update(company);
        return Task.CompletedTask;
    }
}