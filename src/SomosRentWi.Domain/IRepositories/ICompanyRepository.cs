using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Domain.IRepositories;

public interface ICompanyRepository
{
    Task<bool> ExistsByNitAsync(string nit);
    Task AddAsync(Company company);
}