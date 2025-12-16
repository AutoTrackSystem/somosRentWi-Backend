using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Domain.IRepositories;

public interface ICarRepository
{
    Task<Car?> GetByIdAsync(int id);
    Task<List<Car>> GetByCompanyIdAsync(int companyId);
    Task<List<Car>> GetAllAsync();
    Task AddAsync(Car car);
    Task UpdateAsync(Car car);
    Task DeleteAsync(Car car);
}
