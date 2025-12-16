using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomosRentWi.Application.Companies.DTOs;
using SomosRentWi.Application.Companies.Interfaces;

namespace SomosRentWi.Api.Controllers;

[ApiController]
[Route("api/admin/companies")]
[Authorize(Roles = "Admin")]
public class AdminCompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public AdminCompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany(
        [FromBody] CreateCompanyRequest request)
    {
        var result = await _companyService.CreateCompanyAsync(request);
        return Ok(result);
    }
}