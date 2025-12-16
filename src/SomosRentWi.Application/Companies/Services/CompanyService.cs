using SomosRentWi.Application.Companies.DTOs;
using SomosRentWi.Application.Companies.Interfaces;
using SomosRentWi.Application.Security;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.Enums;
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
        await _unitOfWork.SaveChangesAsync();

        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new Exception("Email already exists");

        if (await _companyRepository.ExistsByNitAsync(request.NitNumber))
            throw new Exception("Company already exists");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.Company,
            IsActive = true
        };

        await _userRepository.AddAsync(user);

        var company = new Company
        {
            UserId = user.Id,
            TradeName = request.TradeName,
            NitNumber = request.NitNumber,
            CompanyPlan = request.CompanyPlan,
            SubscriptionStatus = CompanySubscriptionStatus.Active
        };

        await _companyRepository.AddAsync(company);

        await _unitOfWork.SaveChangesAsync();

        return new CompanyResponse
        {
            Id = company.Id,
            TradeName = company.TradeName,
            NitNumber = company.NitNumber
        };
    }
}