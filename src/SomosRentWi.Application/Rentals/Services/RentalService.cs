using SomosRentWi.Application.Rentals.DTOs;
using SomosRentWi.Application.Rentals.Interfaces;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.Exceptions;
using SomosRentWi.Domain.Enums;
using SomosRentWi.Domain.IRepositories;
// using SomosRentWi.Application.Companies.Interfaces;

namespace SomosRentWi.Application.Rentals.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RentalService(
        IRentalRepository rentalRepository,
        ICarRepository carRepository,
        IClientRepository clientRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
        _clientRepository = clientRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RentalResponse> CreateRentalAsync(CreateRentalRequest request, int userId)
    {
        // 1. Validate Business Rules
        if (request.StartDate < DateTime.UtcNow.AddMinutes(-10)) // Allow small buffer
            throw new Exception("Start date cannot be in the past");

        if (request.StartDate >= request.EstimatedEndDate)
            throw new Exception("End date must be after start date");

        // Verify client exists
        var client = await _clientRepository.GetByUserIdAsync(userId);
        if (client == null)
            throw new DomainException("Client not found for this user");

        var clientId = client.Id; // Use resolved ClientId

        // Verify client is verified
        if (client.VerificationStatus != ClientVerificationStatus.Accepted)
            throw new Exception("Client must be verified to rent a car");

        // Verify car exists
        var car = await _carRepository.GetByIdAsync(request.CarId);
        if (car == null)
            throw new Exception("Car not found");

        // 2. Availability Check (Overlap)
        var isOverlapping = await _rentalRepository.HasOverlappingRentalAsync(
            request.CarId, 
            request.StartDate, 
            request.EstimatedEndDate);

        if (isOverlapping)
            throw new Exception("Car is not available for the selected dates");

        // 3. Price Calculation Snapshot
        var pricePerHour = car.PricePerHour; // Snapshot!
        var totalPrice = (decimal)(request.EstimatedEndDate - request.StartDate).TotalHours * pricePerHour;
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
            EndDate = request.EstimatedEndDate, // Set estimated end date
            PricePerHour = pricePerHour, // Store snapshot
            TotalPrice = totalPrice,
            DepositAmount = depositAmount,
            Status = RentalStatus.PendingDelivery
        };

        // Note: We DO NOT set Car.Status = InUse here. 
        // Availability is determined by the HasOverlappingRentalAsync query.
        // Car status 'InUse' should only be set when physically delivered.

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

    public async Task<List<RentalResponse>> GetRentalsByClientIdAsync(int userId)
    {
        var client = await _clientRepository.GetByUserIdAsync(userId);
        if (client == null)
            throw new DomainException("Client not found for this user");

        var rentals = await _rentalRepository.GetByClientIdAsync(client.Id);
        return rentals.Select(MapToResponse).ToList();
    }

    public async Task<List<RentalResponse>> GetRentalsByCompanyIdAsync(int userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null)
            throw new DomainException("Company not found for this user");

        var rentals = await _rentalRepository.GetByCompanyIdAsync(company.Id);
        return rentals.Select(MapToResponse).ToList();
    }

    public async Task<RentalResponse> DeliverRentalAsync(int rentalId, int userId)
    {
        // Resolve company
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null)
            throw new DomainException("Company not found for this user");

        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null)
            throw new DomainException("Rental not found");

        if (rental.CompanyId != company.Id)
            throw new DomainException("Unauthorized: Rental does not belong to your company");

        if (rental.Status != RentalStatus.PendingDelivery)
            throw new Exception("Rental is not pending delivery");

        rental.Status = RentalStatus.InProgress;
        
        // NOW we mark the car as physically InUse
        rental.Car.Status = CarStatus.InUse;
        await _carRepository.UpdateAsync(rental.Car);

        await _rentalRepository.UpdateAsync(rental);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(rental);
    }

    public async Task<RentalResponse> CompleteRentalAsync(int rentalId, int userId)
    {
        // Resolve company
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company == null)
            throw new DomainException("Company not found for this user");

        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null)
            throw new DomainException("Rental not found");

        if (rental.CompanyId != company.Id)
            throw new DomainException("Unauthorized: Rental does not belong to your company");

        if (rental.Status != RentalStatus.InProgress)
            throw new Exception("Rental is not active");

        rental.Status = RentalStatus.FinishedCorrect;
        var actualEndDate = DateTime.UtcNow;
        rental.EndDate = actualEndDate;

        // Calculate actual hours and adjust price using SNAPSHOT price
        var actualHours = (actualEndDate - rental.StartDate).TotalHours;
        // Use Math.Max to ensure at least 1 hour charged or minimum logic if needed
        if (actualHours < 0) actualHours = 0; 
        
        rental.TotalPrice = (decimal)actualHours * rental.PricePerHour; // Use snapshot!

        // Update car status back to available
        rental.Car.Status = CarStatus.Available;
        await _carRepository.UpdateAsync(rental.Car);

        // Process Financial Transaction
        if (rental.Company.Wallet == null)
            throw new Exception("Company wallet not found"); 

        // 1. Calculate Split
        var commissionPercentage = 0.10m;
        var commissionAmount = rental.TotalPrice * commissionPercentage;
        var companyNetIncome = rental.TotalPrice - commissionAmount;

        // 2. Get Platform Wallet (SomosRentWi)
        // Using a fixed NIT for the Platform Company. Ideally from config "Platform:Nit".
        var platformNit = "999999999-PLATFORM"; 
        var platformCompany = await _companyRepository.GetByNitAsync(platformNit);
        
        if (platformCompany == null || platformCompany.Wallet == null)
        {
            // Fallback: If platform wallet doesn't exist, we might Log Error but still complete rental?
            // Or fail? Business rule says we MUST transfer commission.
            // For now, let's create it on the fly or fail. Failing is safer to force correct setup.
            throw new Exception($"Platform Company/Wallet ({platformNit}) not configured. Cannot process commission.");
        }

        var platformWallet = platformCompany.Wallet;

        // 3. Company Transaction (Net Income)
        var companyTransaction = new WalletTransaction
        {
            CompanyWalletId = rental.Company.Wallet.Id,
            Amount = companyNetIncome,
            Description = $"Payment for rental #{rental.Id} (Net Income) - Car: {rental.Car.Brand} {rental.Car.Model}",
            TransactionType = WalletTransactionType.RentalIncome,
            TransactionDate = DateTime.UtcNow
        };

        rental.Company.Wallet.Balance += companyNetIncome;
        rental.Company.Wallet.Transactions.Add(companyTransaction);

        // 4. Platform Transaction (Commission)
        var platformTransaction = new WalletTransaction
        {
            CompanyWalletId = platformWallet.Id,
            Amount = commissionAmount,
            Description = $"Commission (10%) for rental #{rental.Id} - From: {rental.Company.TradeName}",
            TransactionType = WalletTransactionType.CommissionIncome,
            TransactionDate = DateTime.UtcNow
        };

        platformWallet.Balance += commissionAmount;
        // platformWallet.Transactions.Add(platformTransaction); // Need to assure collection is loaded?
        // If GetByNitAsync includes Wallet, EF Core might not have loaded Transactions collection if eager loading wasn't explicit or lazy loading is off.
        // It's safer to just add it to the Context if we are unsure, OR relying on the navigation property if we initialized the list in entity.
        // Entity defines: public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
        // So it is safe to Add to the list even if it was empty from DB (it would be empty list if not loaded? No, EF nulls navigation if not loaded usually vs initialized. But let's check CompanyWallet constructor.
        // CompanyWallet initializes it. BUT EF overrides it.
        // Actually, if we just modify Balance and Add to context via the parent, it should be fine.
        // Safest approach with EF Core when collection loading is uncertain:
        // Use the Repository to Add the transaction entity explicitly? No, that breaks aggregate pattern.
        // We will assume UnitOfWork saves changes to platformWallet because it is tracked.
        
        // However, to be 100% safe about the "Transactions" list being non-null:
        if (platformWallet.Transactions == null) platformWallet.Transactions = new List<WalletTransaction>(); 
        platformWallet.Transactions.Add(platformTransaction);

        await _rentalRepository.UpdateAsync(rental);
        // Note: We modified Company and PlatformCompany wallets. UnitOfWork.SaveChanges will persist all tracked entities.
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
