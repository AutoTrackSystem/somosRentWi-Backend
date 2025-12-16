using SomosRentWi.Application.Auth.DTOs;

namespace SomosRentWi.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterClientAsync(RegisterClientRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
}