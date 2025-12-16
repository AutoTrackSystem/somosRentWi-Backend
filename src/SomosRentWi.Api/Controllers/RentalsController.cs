using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomosRentWi.Application.Rentals.DTOs;
using SomosRentWi.Application.Rentals.Interfaces;
using System.Security.Claims;

namespace SomosRentWi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    /// <summary>
    /// Create a new rental (Client only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> CreateRental([FromBody] CreateRentalRequest request)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var result = await _rentalService.CreateRentalAsync(request, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get rental by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetRentalById(int id)
    {
        try
        {
            var rental = await _rentalService.GetRentalByIdAsync(id);
            if (rental == null)
                return NotFound(new { error = "Rental not found" });

            return Ok(rental);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all rentals for authenticated client (Client only)
    /// </summary>
    [HttpGet("my-rentals")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetMyRentals()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var rentals = await _rentalService.GetRentalsByClientIdAsync(userId);
            return Ok(rentals);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all rentals for authenticated company (Company only)
    /// </summary>
    [HttpGet("company-rentals")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> GetCompanyRentals()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var rentals = await _rentalService.GetRentalsByCompanyIdAsync(userId);
            return Ok(rentals);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deliver a rental (Company only)
    /// </summary>
    [HttpPost("{id}/deliver")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> DeliverRental(int id)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var result = await _rentalService.DeliverRentalAsync(id, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Complete a rental (Company only)
    /// </summary>
    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> CompleteRental(int id)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var result = await _rentalService.CompleteRentalAsync(id, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a rental
    /// </summary>
    [HttpPost("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelRental(int id, [FromBody] CancelRentalRequest request)
    {
        try
        {
            var result = await _rentalService.CancelRentalAsync(id, request.Reason);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private int GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                          
        if (string.IsNullOrEmpty(userIdClaim))
            throw new Exception("User not authenticated");

        return int.Parse(userIdClaim);
    }


}

public class CancelRentalRequest
{
    public string Reason { get; set; } = string.Empty;
}
