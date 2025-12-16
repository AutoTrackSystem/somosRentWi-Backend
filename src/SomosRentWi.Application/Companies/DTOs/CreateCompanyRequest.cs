using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Application.Companies.DTOs;

public class CreateCompanyRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string TradeName { get; set; } = string.Empty;
    public string NitNumber { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;
    public string LandlineNumber { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public CompanyPlan CompanyPlan { get; set; }
}