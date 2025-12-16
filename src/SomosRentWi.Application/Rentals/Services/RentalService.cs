using SomosRentWi.Application.Rentals.DTOs;
using SomosRentWi.Application.Rentals.Interfaces;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.Enums;
using SomosRentWi.Domain.IRepositories;

namespace SomosRentWi.Application.Rentals.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RentalService(
        IRentalRepository rentalRepository,
        ICarRepository carRepository,
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RentalResponse> CreateRentalAsync(CreateRentalRequest request, int clientId)
    {
        // Verify client exists
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null)
            throw new Exception("Client not found");

        // Verify client is verified
        if (client.VerificationStatus != ClientVerificationStatus.Accepted)
            throw new Exception("Client must be verified to rent a car");

        // Verify car exists and is available
        var car = await _carRepository.GetByIdAsync(request.CarId);
        if (car == null)
            throw new Exception("Car not found");

        if (car.Status != CarStatus.Available)
            throw new Exception("Car is not available for rent");

        // Calculate total price and deposit
        var totalPrice = request.EstimatedHours * car.PricePerHour;
        var depositAmount = car.CommercialValue * 0.1m; // 10% of commercial value

        var rental = new Rental
        {
            ClientId = clientId,
            Client = client,
            CompanyId = car.CompanyId,
            Company = car.Company!,
            CarId = car.Id,
            Car = car,
            StartDate = request.StartDate,
            TotalPrice = totalPrice,
            DepositAmount = depositAmount,
            Status = RentalStatus.PendingDelivery
        };

        // Update car status
        car.Status = CarStatus.InUse;
        await _carRepository.UpdateAsync(car);

        await _rentalRepository.AddAsync(rental);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(rental);
    }

    public async Task<RentalResponse?> GetRentalByIdAsync(int id)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null)
            return null;

        return MapToResponse(rental);
    }

    public async Task<List<RentalResponse>> GetRentalsByClientIdAsync(int clientId)
    {
        var rentals = await _rentalRepository.GetByClientIdAsync(clientId);
        return rentals.Select(MapToResponse).ToList();
    }

    public async Task<List<RentalResponse>> GetRentalsByCompanyIdAsync(int companyId)
    {
        var rentals = await _rentalRepository.GetByCompanyIdAsync(companyId);
        return rentals.Select(MapToResponse).ToList();
    }

    public async Task<RentalResponse> DeliverRentalAsync(int rentalId, int companyId)
    {
        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null)
            throw new Exception("Rental not found");

        if (rental.CompanyId != companyId)
            throw new Exception("Unauthorized: Rental does not belong to your company");

        if (rental.Status != RentalStatus.PendingDelivery)
            throw new Exception("Rental is not pending delivery");

        rental.Status = RentalStatus.InProgress;
        await _rentalRepository.UpdateAsync(rental);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(rental);
    }

    public async Task<RentalResponse> CompleteRentalAsync(int rentalId, int companyId)
    {
        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null)
            throw new Exception("Rental not found");

        if (rental.CompanyId != companyId)
            throw new Exception("Unauthorized: Rental does not belong to your company");

        if (rental.Status != RentalStatus.InProgress)
            throw new Exception("Rental is not active");

        rental.Status = RentalStatus.FinishedCorrect;
        rental.EndDate = DateTime.UtcNow;

        // Calculate actual hours and adjust price if needed
        var actualHours = (rental.EndDate.Value - rental.StartDate).TotalHours;
        rental.TotalPrice = (decimal)actualHours * rental.Car.PricePerHour;

        // Update car status back to available
        rental.Car.Status = CarStatus.Available;
        await _carRepository.UpdateAsync(rental.Car);

        await _rentalRepository.UpdateAsync(rental);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(rental);
    }

    public async Task<RentalResponse> CancelRentalAsync(int rentalId, string reason)
    {
        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null)
            throw new Exception("Rental not found");

        if (rental.Status == RentalStatus.FinishedCorrect || rental.Status == RentalStatus.FinishedWithIssue)
            throw new Exception("Cannot cancel a completed or already cancelled rental");

        rental.Status = RentalStatus.FinishedWithIssue;
        
        // If car was rented, make it available again
        if (rental.Car.Status == CarStatus.InUse)
        {
            rental.Car.Status = CarStatus.Available;
            await _carRepository.UpdateAsync(rental.Car);
        }

        await _rentalRepository.UpdateAsync(rental);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(rental);
    }

    private RentalResponse MapToResponse(Rental rental)
    {
        return new RentalResponse
        {
            Id = rental.Id,
            ClientId = rental.ClientId,
            ClientName = $"{rental.Client.FirstName} {rental.Client.LastName}",
            ClientEmail = rental.Client.User?.Email ?? "",
            CompanyId = rental.CompanyId,
            CompanyName = rental.Company.TradeName,
            CarId = rental.CarId,
            CarBrand = rental.Car.Brand,
            CarModel = rental.Car.Model,
            CarPlate = rental.Car.Plate,
            CarPhotoUrl = rental.Car.MainPhotoUrl,
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            TotalPrice = rental.TotalPrice,
            DepositAmount = rental.DepositAmount,
            Status = rental.Status,
            ContractPdfUrl = rental.ContractPdfUrl
        };
    }
}
