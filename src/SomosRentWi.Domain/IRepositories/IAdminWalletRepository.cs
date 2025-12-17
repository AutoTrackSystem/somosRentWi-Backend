using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Domain.IRepositories;

public interface IAdminWalletRepository
{
    Task<AdminWallet?> GetAdminWalletAsync();
}
