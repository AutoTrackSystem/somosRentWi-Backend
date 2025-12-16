using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Application.Auth.DTOs;

public class AuthResult
{
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}