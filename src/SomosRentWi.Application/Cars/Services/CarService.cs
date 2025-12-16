using SomosRentWi.Application.Cars.DTOs;
using SomosRentWi.Application.Cars.Interfaces;
using SomosRentWi.Application.Services;
using SomosRentWi.Domain.Exceptions;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.Enums;
using SomosRentWi.Domain.IRepositories;

namespace SomosRentWi.Application.Cars.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IUnitOfWork _unitOfWork;

    public CarService(
        ICarRepository carRepository,
        ICompanyRepository companyRepository,
        ICloudinaryService cloudinaryService,
        IUnitOfWork unitOfWork)
    {
        _carRepository = carRepository;
        _companyRepository = companyRepository;
        _cloudinaryService = cloudinaryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CarResponse> CreateCarAsync(CreateCarRequest request, int userId)
    {
        // Verify company exists for this user
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null)
            throw new DomainException("Company not found for this user");

        var companyId = company.Id; // Use resolved CompanyId

        // Upload photo to Cloudinary if provided
        string photoUrl = string.Empty;
        if (request.MainPhoto != null)
        {
            photoUrl = await _cloudinaryService.UploadPhotoAsync(
                request.MainPhoto, 
                $"car-photos/company-{companyId}");
        }

        // Calculate price per hour (could include business logic like markup)
        var pricePerHour = request.BasePricePerHour;

        var car = new Car
        {
            CompanyId = companyId,
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            Plate = request.Plate.ToUpper(),
            Color = request.Color,
            CommercialValue = request.CommercialValue,
            BasePricePerHour = request.BasePricePerHour,
            PricePerHour = pricePerHour,
            SoatExpirationDate = request.SoatExpirationDate,
            TechExpirationDate = request.TechExpirationDate,
            MainPhotoUrl = photoUrl,
            Status = CarStatus.Available
        };

        await _carRepository.AddAsync(car);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(car, company);
    }

    public async Task<CarResponse?> GetCarByIdAsync(int id)
    {
        var car = await _carRepository.GetByIdAsync(id);
        if (car == null)
            return null;

        return MapToResponse(car, car.Company!);
    }

    public async Task<List<CarResponse>> GetCarsByCompanyIdAsync(int companyId)
    {
        var cars = await _carRepository.GetByCompanyIdAsync(companyId);
        return cars.Select(c => MapToResponse(c, c.Company!)).ToList();
    }

    public async Task<List<CarResponse>> GetCarsByUserIdAsync(int userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null)
            throw new DomainException("Company not found for this user");
            
        return await GetCarsByCompanyIdAsync(company.Id);
    }

    public async Task<List<CarResponse>> GetAllCarsAsync()
    {
        var cars = await _carRepository.GetAllAsync();
        return cars.Select(c => MapToResponse(c, c.Company!)).ToList();
    }

    public async Task<CarResponse> UpdateCarAsync(int id, UpdateCarRequest request, int userId)
    {
        // Resolve company
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null)
            throw new DomainException("Company not found for this user");

        var car = await _carRepository.GetByIdAsync(id);
        if (car == null)
            throw new DomainException("Car not found");

        if (car.CompanyId != company.Id)
            throw new DomainException("Unauthorized: Car does not belong to your company");

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(request.Brand))
            car.Brand = request.Brand;
        
        if (!string.IsNullOrWhiteSpace(request.Model))
            car.Model = request.Model;
        
        if (request.Year.HasValue)
            car.Year = request.Year.Value;
        
        if (!string.IsNullOrWhiteSpace(request.Plate))
            car.Plate = request.Plate.ToUpper();
        
        if (!string.IsNullOrWhiteSpace(request.Color))
            car.Color = request.Color;
        
        if (request.CommercialValue.HasValue)
            car.CommercialValue = request.CommercialValue.Value;
        
        if (request.BasePricePerHour.HasValue)
        {
            car.BasePricePerHour = request.BasePricePerHour.Value;
            car.PricePerHour = request.BasePricePerHour.Value;
        }
        
        if (request.SoatExpirationDate.HasValue)
            car.SoatExpirationDate = request.SoatExpirationDate.Value;
        
        if (request.TechExpirationDate.HasValue)
            car.TechExpirationDate = request.TechExpirationDate.Value;

        // Update photo if provided
        if (request.MainPhoto != null)
        {
            // Delete old photo if exists
            if (!string.IsNullOrWhiteSpace(car.MainPhotoUrl))
            {
                // Extract public ID from URL and delete
                // This is simplified, you may need to store publicId separately
                // await _cloudinaryService.DeletePhotoAsync(publicId);
            }

            car.MainPhotoUrl = await _cloudinaryService.UploadPhotoAsync(
                request.MainPhoto, 
                $"car-photos/company-{company.Id}");
        }

        await _carRepository.UpdateAsync(car);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(car, car.Company!);
    }

    public async Task DeleteCarAsync(int id, int userId)
    {
        // Resolve company
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null)
            throw new DomainException("Company not found for this user");

        var car = await _carRepository.GetByIdAsync(id);
        if (car == null)
            throw new DomainException("Car not found");

        if (car.CompanyId != company.Id)
            throw new DomainException("Unauthorized: Car does not belong to your company");

        await _carRepository.DeleteAsync(car);
        await _unitOfWork.SaveChangesAsync();
    }

    private CarResponse MapToResponse(Car car, Company company)
    {
        return new CarResponse
        {
            Id = car.Id,
            CompanyId = car.CompanyId,
            CompanyName = company.TradeName,
            Brand = car.Brand,
            Model = car.Model,
            Year = car.Year,
            Plate = car.Plate,
            Color = car.Color,
            CommercialValue = car.CommercialValue,
            BasePricePerHour = car.BasePricePerHour,
            PricePerHour = car.PricePerHour,
            SoatExpirationDate = car.SoatExpirationDate,
            TechExpirationDate = car.TechExpirationDate,
            MainPhotoUrl = car.MainPhotoUrl,
            Status = car.Status
        };
    }
}
