using SomosRentWi.Application.Wallets.DTOs;
using SomosRentWi.Application.Wallets.Interfaces;
using SomosRentWi.Domain.IRepositories;

namespace SomosRentWi.Application.Wallets.Services;

public class WalletQueryService : IWalletQueryService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IAdminWalletRepository _adminWalletRepository;

    public WalletQueryService(ICompanyRepository companyRepository, IAdminWalletRepository adminWalletRepository)
    {
        _companyRepository = companyRepository;
        _adminWalletRepository = adminWalletRepository;
    }

    public async Task<WalletDto> GetCompanyWalletAsync(int userId)
    {
        var wallet = await _companyRepository.GetWalletByUserIdAsync(userId);
        
        if (wallet == null)
        {
            return new WalletDto { Balance = 0, Transactions = new List<TransactionDto>() };
        }

        return new WalletDto
        {
            Balance = wallet.Balance,
            Transactions = wallet.Transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Description = t.Description,
                Type = t.TransactionType.ToString(),
                Date = t.TransactionDate
            }).OrderByDescending(t => t.Date).ToList()
        };
    }

    public async Task<WalletDto> GetAdminWalletAsync()
    {
        var wallet = await _adminWalletRepository.GetAdminWalletAsync();
        
        if (wallet == null)
        {
             return new WalletDto { Balance = 0, Transactions = new List<TransactionDto>() };
        }

        return new WalletDto
        {
            Balance = wallet.Balance,
            Transactions = wallet.Transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Description = t.Description,
                Type = t.TransactionType.ToString(),
                Date = null 
            }).OrderByDescending(t => t.Id).ToList()
        };
    }
}
