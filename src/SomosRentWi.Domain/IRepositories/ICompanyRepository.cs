using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Domain.IRepositories;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(int id);
    Task<Company?> GetByUserIdAsync(int userId);
    Task<List<Company>> GetAllAsync();
    Task<bool> ExistsByNitAsync(string nit);
    Task<Company?> GetByNitAsync(string nit);
    Task<CompanyWallet?> GetWalletByUserIdAsync(int userId);
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
}