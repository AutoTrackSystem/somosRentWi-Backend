using SomosRentWi.Application.Cars.DTOs;

namespace SomosRentWi.Application.Cars.Interfaces;

public interface ICarService
{
    Task<CarResponse> CreateCarAsync(CreateCarRequest request, int userId);
    Task<CarResponse?> GetCarByIdAsync(int id);
    Task<List<CarResponse>> GetCarsByCompanyIdAsync(int companyId); // Public access
    Task<List<CarResponse>> GetCarsByUserIdAsync(int userId); // Owner access
    Task<List<CarResponse>> GetAllCarsAsync();
    Task<CarResponse> UpdateCarAsync(int id, UpdateCarRequest request, int userId);
    Task DeleteCarAsync(int id, int userId);
}
