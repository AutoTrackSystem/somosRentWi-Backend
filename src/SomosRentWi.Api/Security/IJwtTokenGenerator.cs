using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Api.Security;

public interface IJwtTokenGenerator
{
    string Generate(int userId, UserRole role);
}