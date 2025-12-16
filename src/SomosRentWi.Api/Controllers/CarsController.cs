using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomosRentWi.Application.Cars.DTOs;
using SomosRentWi.Application.Cars.Interfaces;
using System.Security.Claims;

namespace SomosRentWi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    /// <summary>
    /// Create a new car (Company only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> CreateCar([FromForm] CreateCarRequest request)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var result = await _carService.CreateCarAsync(request, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get car by ID (Public)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCarById(int id)
    {
        try
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
                return NotFound(new { error = "Car not found" });

            return Ok(car);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all cars from a company (Public)
    /// </summary>
    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> GetCarsByCompanyId(int companyId)
    {
        try
        {
            var cars = await _carService.GetCarsByCompanyIdAsync(companyId);
            return Ok(cars);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all cars my company owns (Company only)
    /// </summary>
    [HttpGet("my-cars")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> GetMyCars()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var cars = await _carService.GetCarsByUserIdAsync(userId);
            return Ok(cars);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all cars (Public)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllCars()
    {
        try
        {
            var cars = await _carService.GetAllCarsAsync();
            return Ok(cars);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update a car (Company only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> UpdateCar(int id, [FromForm] UpdateCarRequest request)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var result = await _carService.UpdateCarAsync(id, request, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a car (Company only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            await _carService.DeleteCarAsync(id, userId);
            return Ok(new { message = "Car deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private int GetUserIdFromClaims()
    {
        // Extract UserId from JWT (sub or NameIdentifier)
        // Note: JwtTokenGenerator uses JwtRegisteredClaimNames.Sub ("sub")
        // ASP.NET Core might map "sub" to ClaimTypes.NameIdentifier by default.
        
        var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value 
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                          
        if (string.IsNullOrEmpty(userIdClaim))
            throw new Exception("User not authenticated");

        return int.Parse(userIdClaim);
    }
}
