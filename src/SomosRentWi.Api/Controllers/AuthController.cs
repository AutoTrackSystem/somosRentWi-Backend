using Microsoft.AspNetCore.Mvc;
using SomosRentWi.Api.Security;
using SomosRentWi.Application.Auth.DTOs;
using SomosRentWi.Application.Auth.Interfaces;

namespace SomosRentWi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwt;

    public AuthController(IAuthService authService, IJwtTokenGenerator jwt)
    {
        _authService = authService;
        _jwt = jwt;
    }

    /// <summary>
    /// Register a new client with document photos
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterClientRequest request)
    {
        try
        {
            var result = await _authService.RegisterClientAsync(request);
            var token = _jwt.Generate(result.UserId, result.Role);

            return Ok(new { token, userId = result.UserId, role = result.Role.ToString() });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            var token = _jwt.Generate(result.UserId, result.Role);

            return Ok(new { token, userId = result.UserId, role = result.Role.ToString() });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}