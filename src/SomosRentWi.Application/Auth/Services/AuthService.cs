using SomosRentWi.Application.Auth.DTOs;
using SomosRentWi.Application.Auth.Interfaces;
using SomosRentWi.Application.Security;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.Enums;
using SomosRentWi.Domain.IRepositories;

namespace SomosRentWi.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IClientRepository clientRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _clientRepository = clientRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResult> RegisterClientAsync(RegisterClientRequest request)
    {
        var email = request.Email.Trim().ToLower();

        if (await _userRepository.ExistsByEmailAsync(email))
            throw new Exception("Email already registered");

        var user = new User
        {
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.Client,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var client = new Client
        {
            UserId = user.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            BirthDate = request.BirthDate,
            PrimaryPhone = request.PrimaryPhone,
            Address = request.Address
        };
        
        await _clientRepository.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResult
        {
            UserId = user.Id,
            Role = user.Role
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(
            request.Email.Trim().ToLower()
        );

        if (user == null ||
            !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        if (!user.IsActive)
            throw new Exception("User inactive");

        return new AuthResult
        {
            UserId = user.Id,
            Role = user.Role
        };
    }
}
