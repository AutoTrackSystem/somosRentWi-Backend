using SomosRentWi.Application.Wallets.DTOs;

namespace SomosRentWi.Application.Wallets.Interfaces;

public interface IWalletQueryService
{
    Task<WalletDto> GetCompanyWalletAsync(int userId);
    Task<WalletDto> GetAdminWalletAsync();
}
