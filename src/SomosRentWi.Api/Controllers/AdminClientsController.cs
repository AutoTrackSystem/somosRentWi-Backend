using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomosRentWi.Application.Clients.Interfaces;
using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Api.Controllers;

[ApiController]
[Route("api/admin/clients")]
[Authorize(Roles = "Admin")]
public class AdminClientsController : ControllerBase
{
    private readonly IAdminClientService _adminClientService;

    public AdminClientsController(IAdminClientService adminClientService)
    {
        _adminClientService = adminClientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        try
        {
            var clients = await _adminClientService.GetAllClientsAsync();
            return Ok(clients);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}/verification-status")]
    public async Task<IActionResult> UpdateVerificationStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            await _adminClientService.UpdateClientVerificationStatusAsync(id, request.Status);
            return Ok(new { message = "Status updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public class UpdateStatusRequest
    {
        public ClientVerificationStatus Status { get; set; }
    }
}
