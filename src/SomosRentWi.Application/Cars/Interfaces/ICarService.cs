using SomosRentWi.Application.Cars.DTOs;

namespace SomosRentWi.Application.Cars.Interfaces;

public interface ICarService
{
    Task<CarResponse> CreateCarAsync(CreateCarRequest request, int companyId);
    Task<CarResponse?> GetCarByIdAsync(int id);
    Task<List<CarResponse>> GetCarsByCompanyIdAsync(int companyId);
    Task<List<CarResponse>> GetAllCarsAsync();
    Task<CarResponse> UpdateCarAsync(int id, UpdateCarRequest request, int companyId);
    Task DeleteCarAsync(int id, int companyId);
}
