using SomosRentWi.Domain.Entities;

namespace SomosRentWi.Domain.IRepositories;

public interface IRentalRepository
{
    Task<Rental?> GetByIdAsync(int id);
    Task<List<Rental>> GetByClientIdAsync(int clientId);
    Task<List<Rental>> GetByCompanyIdAsync(int companyId);
    Task<List<Rental>> GetByCarIdAsync(int carId);
    Task<List<Rental>> GetAllAsync();
    Task AddAsync(Rental rental);
    Task UpdateAsync(Rental rental);
    Task<bool> HasOverlappingRentalAsync(int carId, DateTime startDate, DateTime endDate);
}
