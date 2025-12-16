using SomosRentWi.Application.Companies.DTOs;
using SomosRentWi.Application.Companies.Interfaces;
using SomosRentWi.Application.Security;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.Enums;
using SomosRentWi.Domain.Exceptions;
using SomosRentWi.Domain.IRepositories;

namespace SomosRentWi.Application.Companies.Services;

public class CompanyService : ICompanyService
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CompanyService(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<CompanyResponse> CreateCompanyAsync(CreateCompanyRequest request)
    {
        // 1. Validation
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new DomainException("Email already exists");

        if (await _companyRepository.ExistsByNitAsync(request.NitNumber))
            throw new DomainException("Company already exists in the system");

        // 2. Create Aggregates
        // Create User (Auth)
        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.Company,
            IsActive = true
        };

        // Create Company linked to User via Object reference (not Id)
        var company = new Company
        {
            User = user, // Link navigation property
            TradeName = request.TradeName,
            NitNumber = request.NitNumber,
            ContactEmail = request.ContactEmail,
            LandlineNumber = request.LandlineNumber,
            MobilePhone = request.MobilePhone,
            Address = request.Address,
            Website = request.Website,
            CompanyPlan = request.CompanyPlan,
            SubscriptionStatus = CompanySubscriptionStatus.Active,
            StartSubscriptionDate = DateTime.UtcNow
        };

        // Create Wallet linked to Company
        var wallet = new CompanyWallet
        {
            Balance = 0,
            Company = company // Explicit bi-directional link helps
        };
        
        company.Wallet = wallet;

        // 3. Add to Repository (Graph)
        // Adding the root (Company) is sufficient if proper links exist, 
        // but adding User explicitly is also fine and clearer for intent.
        await _userRepository.AddAsync(user);
        await _companyRepository.AddAsync(company);
        
        // 4. Atomic Commit
        // The UnitOfWork.SaveChangesAsync() call below commits all changes (User, Company, Wallet) 
        // in a single database transaction because they are all added to the same context.
        // This satisfies the atomic invariant: Company cannot exist without User and Wallet.
        await _unitOfWork.SaveChangesAsync();

        return new CompanyResponse
        {
            Id = company.Id,
            TradeName = company.TradeName,
            NitNumber = company.NitNumber
        };
    }
}