using Microsoft.EntityFrameworkCore;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.IRepositories;
using SomosRentWi.Infrastructure.Persistence;

namespace SomosRentWi.Infrastructure.Repositories;

public class AdminWalletRepository : IAdminWalletRepository
{
    private readonly RentWiDbContext _context;

    public AdminWalletRepository(RentWiDbContext context)
    {
        _context = context;
    }

    public async Task<AdminWallet?> GetAdminWalletAsync()
    {
        // Assuming there is only one Admin Wallet, typically Id=1
        // We will fetch the first one found or specifically Id 1
        return await _context.AdminWallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync();
    }
}
