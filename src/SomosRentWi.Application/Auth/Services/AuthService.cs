using SomosRentWi.Application.Auth.DTOs;
using SomosRentWi.Application.Auth.Interfaces;
using SomosRentWi.Application.Security;
using SomosRentWi.Application.Services;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.Enums;
using SomosRentWi.Domain.IRepositories;

namespace SomosRentWi.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IClientRepository clientRepository,
        IPasswordHasher passwordHasher,
        ICloudinaryService cloudinaryService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _clientRepository = clientRepository;
        _passwordHasher = passwordHasher;
        _cloudinaryService = cloudinaryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResult> RegisterClientAsync(RegisterClientRequest request)
    {
        var email = request.Email.Trim().ToLower();

        if (await _userRepository.ExistsByEmailAsync(email))
            throw new Exception("Email already registered");

        // Validate that all required photos are provided
        if (request.SelfiePhoto == null)
            throw new Exception("Selfie photo is required");
        if (request.DocumentFront == null)
            throw new Exception("Document front photo is required");
        if (request.DocumentBack == null)
            throw new Exception("Document back photo is required");
        if (request.LicenseFront == null)
            throw new Exception("License front photo is required");
        if (request.LicenseBack == null)
            throw new Exception("License back photo is required");

        var user = new User
        {
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.Client,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Upload photos to Cloudinary
        var folderPath = $"client-documents/client-{user.Id}";
        
        var selfieUrl = await _cloudinaryService.UploadPhotoAsync(request.SelfiePhoto, folderPath);
        var documentFrontUrl = await _cloudinaryService.UploadPhotoAsync(request.DocumentFront, folderPath);
        var documentBackUrl = await _cloudinaryService.UploadPhotoAsync(request.DocumentBack, folderPath);
        var licenseFrontUrl = await _cloudinaryService.UploadPhotoAsync(request.LicenseFront, folderPath);
        var licenseBackUrl = await _cloudinaryService.UploadPhotoAsync(request.LicenseBack, folderPath);

        var client = new Client
        {
            UserId = user.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            BirthDate = request.BirthDate,
            PrimaryPhone = request.PrimaryPhone,
            Address = request.Address,
            UrlSelfiePhoto = selfieUrl,
            UrlDocumentFront = documentFrontUrl,
            UrlDocumentBack = documentBackUrl,
            UrlLicenseFront = licenseFrontUrl,
            UrlLicenseBack = licenseBackUrl,
            VerificationStatus = ClientVerificationStatus.Pending,
            HistoryStatus = ClientHistoryStatus.NoHistory
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
