using SomosRentWi.Application.Companies.DTOs;

namespace SomosRentWi.Application.Companies.Interfaces;

public interface ICompanyService
{
    Task<CompanyResponse> CreateCompanyAsync(CreateCompanyRequest request);
}